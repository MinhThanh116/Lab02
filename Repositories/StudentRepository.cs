using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using DapperApi.Models;

namespace DapperApi.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly string _connStr;
    
    public StudentRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection NewConnection() => new MySqlConnection(_connStr);

    public IEnumerable<Student> GetAll()
    {
        using var db = NewConnection();
        return db.Query<Student>("SELECT * FROM Students");
    }

    public Student? GetById(int id)
    {
        using var db = NewConnection();
        return db.QuerySingleOrDefault<Student>(
            "SELECT * FROM Students WHERE Id = @Id", new { Id = id });
    }

    // Thêm trường Email vào câu lệnh INSERT
    public void Create(Student student)
    {
        using var db = NewConnection();
        db.Execute(
            "INSERT INTO Students (Name, Age, Email) VALUES (@Name, @Age, @Email)",
            student);
    }

    // Thêm trường Email vào câu lệnh UPDATE
    public void Update(Student student)
    {
        using var db = NewConnection();
        db.Execute(
            "UPDATE Students SET Name=@Name, Age=@Age, Email=@Email WHERE Id=@Id",
            student);
    }

    public void Delete(int id)
    {
        using var db = NewConnection();
        db.Execute(
            "DELETE FROM Students WHERE Id=@Id",
            new { Id = id });
    }

    // Phương thức tìm kiếm theo tên
    public IEnumerable<Student> GetByName(string name)
    {
        using var db = NewConnection();
        // Sử dụng LIKE để tìm kiếm tương đối
        return db.Query<Student>(
            "SELECT * FROM Students WHERE Name LIKE @Name", 
            new { Name = $"%{name}%" });
    }

    public IEnumerable<StudentWithCourses> GetAllWithCourses()
    {
        var sql = @"
            SELECT s.Id, s.Name, s.Age, s.Email, 
                c.Id, c.CourseName
            FROM Students s
            LEFT JOIN StudentCourses sc ON s.Id = sc.StudentId
            LEFT JOIN Courses c ON sc.CourseId = c.Id
            ORDER BY s.Id";

        using var db = NewConnection();
        var dict = new Dictionary<int, StudentWithCourses>();

        db.Query<StudentWithCourses, Course, StudentWithCourses>(
            sql,
            (student, course) =>
            {
                if (!dict.TryGetValue(student.Id, out var existing))
                {
                    existing = student;
                    existing.Courses = new List<Course>();
                    dict[student.Id] = existing;
                }

                if (course != null)
                {
                    existing.Courses.Add(course);
                }

                return existing;
            },
            splitOn: "Id"
        );

        return dict.Values;
    }
}