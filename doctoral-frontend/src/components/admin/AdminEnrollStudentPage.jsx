import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminEnrollStudentPage() {
  const { courseId } = useParams();
  const navigate = useNavigate();
  
  const [course, setCourse] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [students, setStudents] = useState([]);
  const [searchLoading, setSearchLoading] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState(null);
  const [enrolling, setEnrolling] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    fetchCourseDetails();
  }, [courseId]);

  const fetchCourseDetails = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getCourseById(courseId);
      setCourse(data);
    } catch (err) {
      setError(`Failed to load course details: ${err.response?.data?.message || err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const handleSearchStudents = async (e) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      setError('Please enter a student index or name');
      return;
    }

    try {
      setSearchLoading(true);
      setError('');
      setSuccessMessage('');
      const results = await studentService.searchStudents(searchTerm);
      setStudents(Array.isArray(results) ? results : []);
      
      if (results.length === 0) {
        setError('No students found matching your search');
      }
    } catch (err) {
      setError(`Failed to search students: ${err.response?.data?.message || err.message}`);
      setStudents([]);
    } finally {
      setSearchLoading(false);
    }
  };

  const handleEnrollStudent = async (studentId) => {
    try {
      setEnrolling(true);
      setError('');
      
      const result = await studentService.enrollStudentInCourse(studentId, courseId);
      
      setSuccessMessage(`✅ ${selectedStudent.fullName} successfully enrolled in ${course.title}`);
      setSelectedStudent(null);
      setSearchTerm('');
      setStudents([]);
      
      setTimeout(() => {
        navigate('/admin/courses');
      }, 2000);
    } catch (err) {
      setError(`Failed to enroll student: ${err.response?.data?.message || err.message}`);
    } finally {
      setEnrolling(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return 'Invalid Date';
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '900px', margin: '0 auto', textAlign: 'center', paddingTop: '3rem' }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
          <p style={{ color: '#6b7280' }}>Loading course details...</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link
            to="/admin/courses"
            style={{
              color: '#0d9488',
              textDecoration: 'none',
              fontSize: '0.875rem',
              fontWeight: '500',
              marginBottom: '1rem',
              display: 'inline-block',
            }}
          >
            ← Back to Courses
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Enroll Student in Course
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Search and select a student to enroll in this course
          </p>
        </div>

        {/* Course Information */}
        {course && (
          <div
            style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
              border: '1px solid #e5e7eb',
              padding: '1.5rem',
              marginBottom: '2rem',
            }}
          >
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1rem' }}>
              Course Information
            </h2>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem' }}>
              <div>
                <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                  Course Code
                </label>
                <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0, fontWeight: '500' }}>
                  {course.code}
                </p>
              </div>

              <div>
                <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                  Semester
                </label>
                <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0, fontWeight: '500' }}>
                  Semester {course.semester}
                </p>
              </div>

              <div style={{ gridColumn: '1 / -1' }}>
                <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                  Course Title
                </label>
                <p style={{ fontSize: '1.1rem', color: '#1f2937', margin: 0, fontWeight: '500' }}>
                  {course.title}
                </p>
              </div>

              <div>
                <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                  Instructor
                </label>
                <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                  {course.instructorName}
                </p>
              </div>

              <div>
                <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                  ECTS Credits
                </label>
                <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0, fontWeight: '600' }}>
                  {course.ectsCredits}
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Error Message */}
        {error && (
          <div
            style={{
              backgroundColor: '#fef2f2',
              border: '1px solid #fecaca',
              color: '#b91c1c',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1.5rem',
            }}
          >
            {error}
          </div>
        )}

        {/* Success Message */}
        {successMessage && (
          <div
            style={{
              backgroundColor: '#f0fdf4',
              border: '1px solid #86efac',
              color: '#15803d',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1.5rem',
            }}
          >
            {successMessage}
          </div>
        )}

        {/* Search and Selection */}
        <div
          style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}
        >
          <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1.5rem' }}>
            Select Student
          </h2>

          {/* Search Form */}
          <form onSubmit={handleSearchStudents} style={{ marginBottom: '2rem' }}>
            <div style={{ display: 'flex', gap: '1rem', marginBottom: '1rem' }}>
              <input
                type="text"
                placeholder="Search by student index, name, or email..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{
                  flex: 1,
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '1rem',
                  boxSizing: 'border-box',
                }}
              />
              <button
                type="submit"
                disabled={searchLoading}
                style={{
                  backgroundColor: searchLoading ? '#6b7280' : '#0d9488',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: searchLoading ? 'not-allowed' : 'pointer',
                  fontWeight: '500',
                  whiteSpace: 'nowrap',
                }}
              >
                {searchLoading ? 'Searching...' : '🔍 Search'}
              </button>
            </div>
            <p style={{ fontSize: '0.875rem', color: '#6b7280', margin: '0.5rem 0 0 0' }}>
              Enter a student index number, full name, or email address to find the student.
            </p>
          </form>

          {/* Selected Student */}
          {selectedStudent && (
            <div
              style={{
                backgroundColor: '#f0fdf4',
                border: '2px solid #86efac',
                borderRadius: '0.5rem',
                padding: '1.5rem',
                marginBottom: '1.5rem',
              }}
            >
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                <div>
                  <h3 style={{ fontSize: '1.1rem', fontWeight: 'bold', color: '#1f2937', margin: '0 0 0.5rem 0' }}>
                    ✅ {selectedStudent.fullName}
                  </h3>
                  <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', fontSize: '0.875rem' }}>
                    <div>
                      <span style={{ color: '#6b7280' }}>Student Index: </span>
                      <span style={{ fontWeight: '600', color: '#1f2937' }}>{selectedStudent.indexNumber}</span>
                    </div>
                    <div>
                      <span style={{ color: '#6b7280' }}>Email: </span>
                      <span style={{ fontWeight: '600', color: '#1f2937' }}>{selectedStudent.email}</span>
                    </div>
                    {selectedStudent.faculty && (
                      <div>
                        <span style={{ color: '#6b7280' }}>Faculty: </span>
                        <span style={{ fontWeight: '600', color: '#1f2937' }}>{selectedStudent.faculty}</span>
                      </div>
                    )}
                  </div>
                </div>
                <button
                  onClick={() => setSelectedStudent(null)}
                  style={{
                    backgroundColor: 'transparent',
                    color: '#ef4444',
                    border: '1px solid #ef4444',
                    padding: '0.5rem 1rem',
                    borderRadius: '0.5rem',
                    cursor: 'pointer',
                    fontWeight: '500',
                    fontSize: '0.875rem',
                  }}
                >
                  Deselect
                </button>
              </div>
            </div>
          )}

          {/* Student Results */}
          {students.length > 0 && !selectedStudent && (
            <div style={{ marginBottom: '1.5rem' }}>
              <h3 style={{ fontSize: '1rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
                Search Results ({students.length})
              </h3>
              <div style={{ display: 'grid', gap: '0.75rem' }}>
                {students.map((student) => (
                  <button
                    key={student.id}
                    onClick={() => setSelectedStudent(student)}
                    style={{
                      width: '100%',
                      textAlign: 'left',
                      padding: '1rem',
                      border: '1px solid #d1d5db',
                      borderRadius: '0.5rem',
                      backgroundColor: 'white',
                      cursor: 'pointer',
                      transition: 'all 0.2s',
                      display: 'flex',
                      justifyContent: 'space-between',
                      alignItems: 'center',
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.backgroundColor = '#f9fafb';
                      e.currentTarget.style.borderColor = '#0d9488';
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.backgroundColor = 'white';
                      e.currentTarget.style.borderColor = '#d1d5db';
                    }}
                  >
                    <div>
                      <div style={{ fontWeight: '600', color: '#1f2937', marginBottom: '0.25rem' }}>
                        {student.fullName}
                      </div>
                      <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                        Index: {student.indexNumber} • {student.email}
                      </div>
                      {student.faculty && (
                        <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>
                          {student.faculty}
                        </div>
                      )}
                    </div>
                    <div style={{ color: '#0d9488', fontWeight: '600' }}>Select →</div>
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* Empty State */}
          {searchTerm && students.length === 0 && !searchLoading && !selectedStudent && (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '2rem' }}>
              <div style={{ fontSize: '2rem', marginBottom: '1rem' }}>🔍</div>
              <p>No students found. Try adjusting your search.</p>
            </div>
          )}

          {/* Enrollment Action */}
          {selectedStudent && (
            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', paddingTop: '1.5rem', borderTop: '1px solid #e5e7eb' }}>
              <button
                onClick={() => setSelectedStudent(null)}
                style={{
                  backgroundColor: '#e5e7eb',
                  color: '#1f2937',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: 'pointer',
                  fontWeight: '500',
                  fontSize: '0.95rem',
                }}
              >
                Cancel
              </button>
              <button
                onClick={() => handleEnrollStudent(selectedStudent.id)}
                disabled={enrolling}
                style={{
                  backgroundColor: enrolling ? '#6b7280' : '#10b981',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: enrolling ? 'not-allowed' : 'pointer',
                  fontWeight: '500',
                  fontSize: '0.95rem',
                }}
              >
                {enrolling ? 'Enrolling...' : '✅ Confirm Enrollment'}
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}