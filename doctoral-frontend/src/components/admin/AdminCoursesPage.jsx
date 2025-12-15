import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminCoursesPage() {
  const [courses, setCourses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [semesterFilter, setSemesterFilter] = useState('');
  const [instructorFilter, setInstructorFilter] = useState('');

  useEffect(() => {
    fetchCourses();
  }, []);

  const fetchCourses = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getCourses();
      setCourses(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to fetch courses:', err);
      setError('Failed to load courses');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id, title) => {
    if (!window.confirm(`Are you sure you want to delete "${title}"? This action cannot be undone.`)) {
      return;
    }

    try {
      await studentService.deleteCourse(id);
      setCourses(courses.filter(c => c.id !== id));
      alert('Course deleted successfully');
    } catch (err) {
      console.error('Failed to delete course:', err);
      setError('Failed to delete course');
    }
  };

  const filteredCourses = courses.filter(course =>
    (course.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
     course.title.toLowerCase().includes(searchTerm.toLowerCase())) &&
    (semesterFilter === '' || course.semester.toString() === semesterFilter) &&
    (instructorFilter === '' || course.instructorName.toLowerCase().includes(instructorFilter.toLowerCase()))
  );

  const uniqueSemesters = [...new Set(courses.map(c => c.semester))].sort((a, b) => a - b);

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
                Manage Courses
              </h1>
              <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
                View, edit, and manage all doctoral courses
              </p>
            </div>
            <Link
              to="/admin/courses/new"
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                textDecoration: 'none',
                fontWeight: '500',
              }}
            >
              + Add New Course
            </Link>
          </div>
        </div>

        {error && (
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
          }}>
            {error}
          </div>
        )}

        {/* Filters */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h3 style={{ fontSize: '1rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
            Filters
          </h3>
          
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '1rem' }}>
            {/* Search */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Search (Code/Title)
              </label>
              <input
                type="text"
                placeholder="Search courses..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                }}
              />
            </div>

            {/* Semester Filter */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Semester
              </label>
              <select
                value={semesterFilter}
                onChange={(e) => setSemesterFilter(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  backgroundColor: 'white',
                  cursor: 'pointer',
                }}
              >
                <option value="">All Semesters</option>
                {uniqueSemesters.map((sem) => (
                  <option key={sem} value={sem}>
                    Semester {sem}
                  </option>
                ))}
              </select>
            </div>

            {/* Instructor Filter */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Instructor
              </label>
              <input
                type="text"
                placeholder="Filter by instructor..."
                value={instructorFilter}
                onChange={(e) => setInstructorFilter(e.target.value)}
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                }}
              />
            </div>
          </div>

          <button
            onClick={() => {
              setSearchTerm('');
              setSemesterFilter('');
              setInstructorFilter('');
            }}
            style={{
              backgroundColor: '#e5e7eb',
              color: '#1f2937',
              padding: '0.5rem 1rem',
              borderRadius: '0.5rem',
              border: 'none',
              cursor: 'pointer',
              fontWeight: '500',
              fontSize: '0.875rem',
            }}
          >
            Reset Filters
          </button>
        </div>

        {/* Courses Table */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <div style={{ marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Courses ({filteredCourses.length})
            </h2>
          </div>

          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading courses...</p>
            </div>
          ) : filteredCourses.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📖</div>
              <p>No courses found.</p>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>ID</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Code</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Title</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Instructor</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Semester</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>ECTS Credits</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredCourses.map((course) => (
                    <tr
                      key={course.id}
                      style={{
                        borderBottom: '1px solid #e5e7eb',
                        transition: 'background-color 0.2s',
                      }}
                      onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = '#f9fafb';
                      }}
                      onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = 'transparent';
                      }}
                    >
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{course.id}</td>
                      <td style={{ padding: '0.75rem', fontWeight: '600', color: '#1f2937' }}>{course.code}</td>
                      <td style={{ padding: '0.75rem', fontWeight: '500', color: '#1f2937' }}>{course.title}</td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{course.instructorName}</td>
                      <td style={{ padding: '0.75rem' }}>
                        <span style={{
                          display: 'inline-block',
                          padding: '0.25rem 0.75rem',
                          borderRadius: '9999px',
                          backgroundColor: '#dbeafe',
                          color: '#1e40af',
                          fontWeight: '500',
                          fontSize: '0.75rem',
                        }}>
                          Sem {course.semester}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem', fontWeight: '500', color: '#1f2937' }}>{course.ectsCredits}</td>
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          <Link
                            to={`/admin/courses/${course.id}/edit`}
                            style={{
                              backgroundColor: '#3b82f6',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              textDecoration: 'none',
                              fontWeight: '500',
                              fontSize: '0.875rem',
                            }}
                          >
                            Edit
                          </Link>
                          <Link
                            to={`/admin/courses/${course.id}/enroll`}
                            style={{
                              backgroundColor: '#10b981',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              textDecoration: 'none',
                              fontWeight: '500',
                              fontSize: '0.875rem',
                            }}
                          >
                            Enroll
                          </Link>

                          <button
                            onClick={() => handleDelete(course.id, course.title)}
                            style={{
                              backgroundColor: '#ef4444',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              border: 'none',
                              cursor: 'pointer',
                              fontWeight: '500',
                              fontSize: '0.875rem',
                            }}
                          >
                            Delete
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
