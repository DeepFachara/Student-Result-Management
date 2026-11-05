using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace StudentResultManagement.Models
{
    public static class DatabaseHelper
    {
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["StudentResultDB"].ConnectionString;

        public static Student GetStudent(string enrollmentNo, string password)
        {
            Student student = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT StudentID, EnrollmentNo, Name, Email,
                                        Password, Department, Semester
                                 FROM Students
                                 WHERE LOWER(LTRIM(RTRIM(EnrollmentNo))) = LOWER(LTRIM(RTRIM(@EnrollmentNo)))
                                 AND LTRIM(RTRIM(Password)) = LTRIM(RTRIM(@Password))";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentNo", enrollmentNo);
                    cmd.Parameters.AddWithValue("@Password", password);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new Student
                            {
                                StudentID = (int)reader["StudentID"],
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Department = reader["Department"].ToString(),
                                Semester = (int)reader["Semester"]
                            };
                        }
                    }
                }
            }

            return student;
        }

        public static Student GetStudentById(int studentId)
        {
            Student student = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT StudentID, EnrollmentNo, Name, Email,
                                        Password, Department, Semester
                                 FROM Students
                                 WHERE StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            student = new Student
                            {
                                StudentID = (int)reader["StudentID"],
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Department = reader["Department"].ToString(),
                                Semester = (int)reader["Semester"]
                            };
                        }
                    }
                }
            }

            return student;
        }

        public static List<Result> GetStudentResults(int studentId)
        {
            List<Result> results = new List<Result>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                r.ResultID,
                r.StudentID,
                r.SubjectID,
                r.Marks,
                s.Name AS StudentName,
                s.EnrollmentNo,
                sub.SubjectName,
                sub.MaxMarks,
                sub.PassingMarks
            FROM Results r
            INNER JOIN Students s ON r.StudentID = s.StudentID
            INNER JOIN Subjects sub ON r.SubjectID = sub.SubjectID
            WHERE r.StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new Result
                            {
                                ResultID = (int)reader["ResultID"],
                                StudentID = (int)reader["StudentID"],
                                SubjectID = (int)reader["SubjectID"],
                                Marks = (int)reader["Marks"],
                                StudentName = reader["StudentName"].ToString(),
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                SubjectName = reader["SubjectName"].ToString(),
                                MaxMarks = (int)reader["MaxMarks"],
                                PassingMarks = (int)reader["PassingMarks"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public static List<Subject> GetAllSubjects()
        {
            List<Subject> subjects = new List<Subject>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT SubjectID, SubjectName, MaxMarks, PassingMarks
                         FROM Subjects";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjects.Add(new Subject
                            {
                                SubjectID = (int)reader["SubjectID"],
                                SubjectName = reader["SubjectName"].ToString(),
                                MaxMarks = (int)reader["MaxMarks"],
                                PassingMarks = (int)reader["PassingMarks"]
                            });
                        }
                    }
                }
            }

            return subjects;
        }

        public static Subject GetSubjectById(int subjectId)
        {
            Subject subject = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT SubjectID, SubjectName, MaxMarks, PassingMarks
                         FROM Subjects
                         WHERE SubjectID = @SubjectID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            subject = new Subject
                            {
                                SubjectID = (int)reader["SubjectID"],
                                SubjectName = reader["SubjectName"].ToString(),
                                MaxMarks = (int)reader["MaxMarks"],
                                PassingMarks = (int)reader["PassingMarks"]
                            };
                        }
                    }
                }
            }

            return subject;
        }

        public static void AddSubject(Subject subject)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO Subjects
                         (SubjectName, MaxMarks, PassingMarks)
                         VALUES
                         (@SubjectName, @MaxMarks, @PassingMarks)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                    cmd.Parameters.AddWithValue("@MaxMarks", subject.MaxMarks);
                    cmd.Parameters.AddWithValue("@PassingMarks", subject.PassingMarks);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateSubject(Subject subject)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE Subjects
                         SET SubjectName = @SubjectName,
                             MaxMarks = @MaxMarks,
                             PassingMarks = @PassingMarks
                         WHERE SubjectID = @SubjectID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SubjectID", subject.SubjectID);
                    cmd.Parameters.AddWithValue("@SubjectName", subject.SubjectName);
                    cmd.Parameters.AddWithValue("@MaxMarks", subject.MaxMarks);
                    cmd.Parameters.AddWithValue("@PassingMarks", subject.PassingMarks);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteSubject(int subjectId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Delete related results first
                string deleteResults = @"DELETE FROM Results
                                  WHERE SubjectID = @SubjectID";

                using (SqlCommand cmd = new SqlCommand(deleteResults, con))
                {
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                    cmd.ExecuteNonQuery();
                }

                string deleteSubject = @"DELETE FROM Subjects
                                 WHERE SubjectID = @SubjectID";

                using (SqlCommand cmd = new SqlCommand(deleteSubject, con))
                {
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    

    public static List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT StudentID, EnrollmentNo, Name, Email,
                                Password, Department, Semester
                         FROM Students";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student
                            {
                                StudentID = (int)reader["StudentID"],
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Department = reader["Department"].ToString(),
                                Semester = (int)reader["Semester"]
                            });
                        }
                    }
                }
            }

            return students;
        }

        public static List<Result> GetAllResults()
        {
            List<Result> results = new List<Result>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                r.ResultID,
                r.StudentID,
                r.SubjectID,
                r.Marks,
                s.Name AS StudentName,
                s.EnrollmentNo,
                sub.SubjectName,
                sub.MaxMarks,
                sub.PassingMarks
            FROM Results r
            INNER JOIN Students s
                ON r.StudentID = s.StudentID
            INNER JOIN Subjects sub
                ON r.SubjectID = sub.SubjectID
            ORDER BY r.ResultID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new Result
                            {
                                ResultID = (int)reader["ResultID"],
                                StudentID = (int)reader["StudentID"],
                                SubjectID = (int)reader["SubjectID"],
                                Marks = (int)reader["Marks"],
                                StudentName = reader["StudentName"].ToString(),
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                SubjectName = reader["SubjectName"].ToString(),
                                MaxMarks = (int)reader["MaxMarks"],
                                PassingMarks = (int)reader["PassingMarks"]
                            });
                        }
                    }
                }
            }

            return results;
        }

        public static Result GetResultById(int resultId)
        {
            Result result = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT
                r.ResultID,
                r.StudentID,
                r.SubjectID,
                r.Marks,
                s.Name AS StudentName,
                s.EnrollmentNo,
                sub.SubjectName,
                sub.MaxMarks,
                sub.PassingMarks
            FROM Results r
            INNER JOIN Students s
                ON r.StudentID = s.StudentID
            INNER JOIN Subjects sub
                ON r.SubjectID = sub.SubjectID
            WHERE r.ResultID = @ResultID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ResultID", resultId);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = new Result
                            {
                                ResultID = (int)reader["ResultID"],
                                StudentID = (int)reader["StudentID"],
                                SubjectID = (int)reader["SubjectID"],
                                Marks = (int)reader["Marks"],
                                StudentName = reader["StudentName"].ToString(),
                                EnrollmentNo = reader["EnrollmentNo"].ToString(),
                                SubjectName = reader["SubjectName"].ToString(),
                                MaxMarks = (int)reader["MaxMarks"],
                                PassingMarks = (int)reader["PassingMarks"]
                            };
                        }
                    }
                }
            }

            return result;
        }

        public static bool ResultExists(int studentId, int subjectId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT COUNT(*)
            FROM Results
            WHERE StudentID = @StudentID
            AND SubjectID = @SubjectID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectId);

                    con.Open();

                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public static void AddResult(Result result)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Results
            (StudentID, SubjectID, Marks)
            VALUES
            (@StudentID, @SubjectID, @Marks)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", result.StudentID);
                    cmd.Parameters.AddWithValue("@SubjectID", result.SubjectID);
                    cmd.Parameters.AddWithValue("@Marks", result.Marks);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateResult(Result result)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE Results
            SET StudentID = @StudentID,
                SubjectID = @SubjectID,
                Marks = @Marks
            WHERE ResultID = @ResultID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ResultID", result.ResultID);
                    cmd.Parameters.AddWithValue("@StudentID", result.StudentID);
                    cmd.Parameters.AddWithValue("@SubjectID", result.SubjectID);
                    cmd.Parameters.AddWithValue("@Marks", result.Marks);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteResult(int resultId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            DELETE FROM Results
            WHERE ResultID = @ResultID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ResultID", resultId);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void AddStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            INSERT INTO Students
            (EnrollmentNo, Name, Email, Password, Department, Semester)
            VALUES
            (@EnrollmentNo, @Name, @Email, @Password, @Department, @Semester)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
                    cmd.Parameters.AddWithValue("@Name", student.Name);
                    cmd.Parameters.AddWithValue("@Email", student.Email);
                    cmd.Parameters.AddWithValue("@Password", student.Password);
                    cmd.Parameters.AddWithValue("@Department", student.Department);
                    cmd.Parameters.AddWithValue("@Semester", student.Semester);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateStudent(Student student)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE Students
            SET EnrollmentNo = @EnrollmentNo,
                Name = @Name,
                Email = @Email,
                Password = @Password,
                Department = @Department,
                Semester = @Semester
            WHERE StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", student.StudentID);
                    cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
                    cmd.Parameters.AddWithValue("@Name", student.Name);
                    cmd.Parameters.AddWithValue("@Email", student.Email);
                    cmd.Parameters.AddWithValue("@Password", student.Password);
                    cmd.Parameters.AddWithValue("@Department", student.Department);
                    cmd.Parameters.AddWithValue("@Semester", student.Semester);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteStudent(int studentId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Delete student's results first
                string deleteResults = @"
            DELETE FROM Results
            WHERE StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(deleteResults, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.ExecuteNonQuery();
                }

                // Then delete the student
                string deleteStudent = @"
            DELETE FROM Students
            WHERE StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(deleteStudent, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}