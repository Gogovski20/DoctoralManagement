import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import CompleteEnrollmentModal from './CompleteEnrollmentModal';


export default function AdminCourseEnrollmentsPage() {
  const [enrollments, setEnrollments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [filterCompleted, setFilterCompleted] = useState('all');
  const [sortBy, setSortBy] = useState('enrolledDate');
  const [selectedEnrollmentForCompletion, setSelectedEnrollmentForCompletion] = useState(null);


  useEffect(() => {
    fetchEnrollments();
  }, []);


  // Debug log when enrollment is selected for completion
  useEffect(() => {
    if (selectedEnrollmentForCompletion) {
      console.log('Selected enrollment for completion:', {
        id: selectedEnrollmentForCompletion.id,
        studentId: selectedEnrollmentForCompletion.studentId,
        enrollmentId: selectedEnrollmentForCompletion.id,
        studentName: selectedEnrollmentForCompletion.studentName,
        courseTitle: selectedEnrollmentForCompletion.courseTitle,
        allProperties: Object.keys(selectedEnrollmentForCompletion),
      });
    }
  }, [selectedEnrollmentForCompletion]);


  const fetchEnrollments = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getAllEnrollments(); // data is already response.data
      
      // Debug log to see enrollment structure
      if (Array.isArray(data) && data.length > 0) {
        console.log('First enrollment object properties:', Object.keys(data[0]));
        console.log('First enrollment object:', data[0]);
      }
      
      setEnrollments(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to fetch enrollments:', err);
      setError(`Failed to load enrollments: ${err.response?.data?.message || err.message}`);
    } finally {
      setLoading(false);
    }
  };


  const handleCompleteSuccess = () => {
    // Refresh the enrollments list
    fetchEnrollments();
    setSelectedEnrollmentForCompletion(null);
  };


  const filteredAndSortedEnrollments = enrollments
    .filter(enrollment => {
      const matchesSearch = 
        enrollment.studentName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        enrollment.studentIndex.toLowerCase().includes(searchTerm.toLowerCase()) ||
        enrollment.courseTitle.toLowerCase().includes(searchTerm.toLowerCase());


      let matchesStatus = true;
      if (filterCompleted === 'completed') {
        matchesStatus = enrollment.completed;
      } else if (filterCompleted === 'pending') {
        matchesStatus = !enrollment.completed;
      }


      return matchesSearch && matchesStatus;
    })
    .sort((a, b) => {
      switch (sortBy) {
        case 'enrolledDate':
          return new Date(b.enrolledDate) - new Date(a.enrolledDate);
        case 'studentName':
          return a.studentName.localeCompare(b.studentName);
        case 'courseTitle':
          return a.courseTitle.localeCompare(b.courseTitle);
        default:
          return 0;
      }
    });


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


  const getGradeColor = (grade) => {
    if (!grade || grade === 0) return '#6b7280';
    if (grade >= 9.0) return '#10b981';
    if (grade >= 7.0) return '#3b82f6';
    if (grade >= 6.0) return '#f59e0b';
    return '#ef4444';
  };


  const getGradeLabel = (grade) => {
    if (!grade || grade === 0) return 'N/A';
    return grade.toFixed(2);
  };


  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Course Enrollments
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              View and manage all student course enrollments
            </p>
          </div>
          <Link
            to="/admin/dashboard"
            style={{
              textDecoration: 'none',
              backgroundColor: '#e5e7eb',
              color: '#1f2937',
              padding: '0.75rem 1.5rem',
              borderRadius: '0.5rem',
              fontWeight: '500',
              fontSize: '0.95rem',
            }}
          >
            ← Back to Dashboard
          </Link>
        </div>


        {/* Error banner */}
        {error && (
          <div
            style={{
              backgroundColor: '#fef2f2',
              border: '1px solid #fecaca',
              color: '#b91c1c',
              padding: '1rem',
              borderRadius: '0.5rem',
              marginBottom: '1rem',
            }}
          >
            {error}
          </div>
        )}


        {/* Filters */}
        <div
          style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
            marginBottom: '1.5rem',
          }}
        >
          <h3 style={{ fontSize: '1rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
            Filters & Search
          </h3>


          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '1rem' }}>
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Search
              </label>
              <input
                type="text"
                placeholder="Search by student, index, or course..."
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


            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Status
              </label>
              <select
                value={filterCompleted}
                onChange={(e) => setFilterCompleted(e.target.value)}
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
                <option value="all">All Enrollments</option>
                <option value="pending">Pending (In Progress)</option>
                <option value="completed">Completed</option>
              </select>
            </div>


            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Sort By
              </label>
              <select
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value)}
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
                <option value="enrolledDate">Enrolled Date (Newest)</option>
                <option value="studentName">Student Name</option>
                <option value="courseTitle">Course Title</option>
              </select>
            </div>
          </div>


          <button
            onClick={() => {
              setSearchTerm('');
              setFilterCompleted('all');
              setSortBy('enrolledDate');
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


        {/* Enrollments Table */}
        <div
          style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}
        >
          <div style={{ marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Enrollments ({filteredAndSortedEnrollments.length} of {enrollments.length})
            </h2>
          </div>


          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading enrollments...</p>
            </div>
          ) : filteredAndSortedEnrollments.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📭</div>
              <p>No enrollments found.</p>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Student
                    </th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Course
                    </th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Enrolled Date
                    </th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Status
                    </th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Grade
                    </th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {filteredAndSortedEnrollments.map((enrollment) => (
                    <tr
                      key={enrollment.id}
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
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937' }}>{enrollment.studentName}</div>
                        <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>{enrollment.studentIndex}</div>
                      </td>
                      <td style={{ padding: '0.75rem', color: '#1f2937', fontWeight: '500' }}>
                        {enrollment.courseTitle}
                      </td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(enrollment.enrolledDate)}
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <span
                          style={{
                            display: 'inline-block',
                            padding: '0.25rem 0.75rem',
                            borderRadius: '9999px',
                            backgroundColor: enrollment.completed ? '#10b98120' : '#f59e0b20',
                            color: enrollment.completed ? '#10b981' : '#f59e0b',
                            fontWeight: '500',
                            fontSize: '0.75rem',
                          }}
                        >
                          {enrollment.completed ? '✅ Completed' : '⏳ In Progress'}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <span
                          style={{
                            display: 'inline-block',
                            padding: '0.25rem 0.75rem',
                            borderRadius: '0.5rem',
                            backgroundColor: getGradeColor(enrollment.grade) + '20',
                            color: getGradeColor(enrollment.grade),
                            fontWeight: '600',
                            fontSize: '0.875rem',
                          }}
                        >
                          {getGradeLabel(enrollment.grade)}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          {!enrollment.completed && (
                            <button
                              onClick={() => {
                                console.log('Enrollment data being sent to modal:', enrollment);
                                setSelectedEnrollmentForCompletion(enrollment);
                              }}
                              style={{
                                backgroundColor: '#10b981',
                                color: 'white',
                                padding: '0.5rem 1rem',
                                borderRadius: '0.5rem',
                                border: 'none',
                                cursor: 'pointer',
                                fontWeight: '500',
                                fontSize: '0.875rem',
                              }}
                            >
                              Complete
                            </button>
                          )}
                          {enrollment.completed && (
                            <span
                              style={{
                                backgroundColor: '#10b98120',
                                color: '#10b981',
                                padding: '0.5rem 1rem',
                                borderRadius: '0.5rem',
                                fontWeight: '500',
                                fontSize: '0.875rem',
                              }}
                            >
                              ✓ Marked Complete
                            </span>
                          )}
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


      {/* Complete Enrollment Modal */}
      {selectedEnrollmentForCompletion && (
        <CompleteEnrollmentModal
          enrollment={selectedEnrollmentForCompletion}
          onClose={() => setSelectedEnrollmentForCompletion(null)}
          onSuccess={handleCompleteSuccess}
        />
      )}
    </div>
  );
}