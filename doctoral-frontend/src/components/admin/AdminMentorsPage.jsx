import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminMentorsPage() {
  const [mentors, setMentors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    fetchMentors();
  }, []);

  const fetchMentors = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getAllMentors();
      setMentors(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to fetch mentors:', err);
      setError('Failed to load mentors');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id, name) => {
    if (!window.confirm(`Are you sure you want to delete ${name}? This action cannot be undone.`)) {
      return;
    }

    try {
      await studentService.deleteMentor(id);
      setMentors(mentors.filter(m => m.id !== id));
      alert('Mentor deleted successfully');
    } catch (err) {
      console.error('Failed to delete mentor:', err);
      setError('Failed to delete mentor');
    }
  };

  const filteredMentors = mentors.filter(mentor =>
    mentor.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mentor.department.toLowerCase().includes(searchTerm.toLowerCase()) ||
    mentor.title.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <div>
              <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
                Manage Mentors
              </h1>
              <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
                View, edit, and manage all mentors
              </p>
            </div>
            <Link
              to="/admin/mentors/new"
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                textDecoration: 'none',
                fontWeight: '500',
              }}
            >
              + Add New Mentor
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

        {/* Search Box */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <input
            type="text"
            placeholder="Search by name, department, or title..."
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

        {/* Mentors Table */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <div style={{ marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Mentors ({filteredMentors.length})
            </h2>
          </div>

          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading mentors...</p>
            </div>
          ) : filteredMentors.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>👨‍🏫</div>
              <p>No mentors found.</p>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>ID</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Full Name</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Title</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Department</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Max Students</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Research Areas</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Status</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredMentors.map((mentor) => (
                    <tr
                      key={mentor.id}
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
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{mentor.id}</td>
                      <td style={{ padding: '0.75rem', fontWeight: '500', color: '#1f2937' }}>{mentor.fullName}</td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{mentor.title}</td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{mentor.department}</td>
                      <td style={{ padding: '0.75rem', fontWeight: '500', color: '#1f2937' }}>{mentor.maxStudents}</td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {mentor.researchAreas && mentor.researchAreas.length > 0
                          ? mentor.researchAreas.slice(0, 2).join(', ') + (mentor.researchAreas.length > 2 ? '...' : '')
                          : 'N/A'
                        }
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <span style={{
                          display: 'inline-block',
                          padding: '0.25rem 0.75rem',
                          borderRadius: '9999px',
                          backgroundColor: mentor.isActive ? '#dcfce7' : '#fee2e2',
                          color: mentor.isActive ? '#166534' : '#991b1b',
                          fontWeight: '500',
                          fontSize: '0.75rem',
                        }}>
                          {mentor.isActive ? '✓ Active' : '✗ Inactive'}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          <Link
                            to={`/admin/mentors/${mentor.id}/edit`}
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
                          <button
                            onClick={() => handleDelete(mentor.id, mentor.fullName)}
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
