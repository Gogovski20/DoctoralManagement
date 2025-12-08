import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminApplicationsPage() {
  const [applications, setApplications] = useState([]);
  const [programs, setPrograms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  // Filters
  const [filters, setFilters] = useState({
    status: '',
    programId: '',
    studentId: '',
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError('');
      
      const [appsData, programsData] = await Promise.all([
        studentService.getAllApplications(filters),
        studentService.getAllPrograms(),
      ]);
      
      setApplications(Array.isArray(appsData) ? appsData : []);
      setPrograms(Array.isArray(programsData) ? programsData : []);
    } catch (err) {
      console.error('Failed to fetch data:', err);
      setError('Failed to load applications');
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    const newFilters = { ...filters, [name]: value };
    setFilters(newFilters);
  };

  const handleApplyFilters = () => {
    fetchData();
  };

  const handleResetFilters = () => {
    setFilters({ status: '', programId: '', studentId: '' });
    setApplications([]);
  };

  const getStatusColor = (status) => {
    if (!status) return '#6b7280';
    const statusStr = status.toString().toLowerCase();
    switch (statusStr) {
      case 'draft':
        return '#6b7280';
      case 'submitted':
        return '#3b82f6';
      case 'underreview':
      case 'under_review':
        return '#f59e0b';
      case 'preliminary_accepted':
      case 'preliminaryaccepted':
        return '#f50ba7ff';    
      case 'approved':
        return '#10b981';
      case 'final_accepted':
      case 'finalaccepted':
        return '#059669';
      case 'rejected':
        return '#ef4444';
      default:
        return '#6b7280';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return 'Draft';
    return status
      .toString()
      .replace(/([A-Z])/g, ' $1')
      .trim()
      .replace(/\b\w/g, l => l.toUpperCase());
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      });
    } catch (e) {
      return 'Invalid Date';
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Review Applications
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Review and manage all doctoral applications
          </p>
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

        {/* Filters Card */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <h2 style={{ fontSize: '1.25rem', fontWeight: '600', color: '#1f2937', marginBottom: '1rem' }}>
            Filters
          </h2>
          
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '1rem', marginBottom: '1rem' }}>
            {/* Status Filter */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Status
              </label>
              <select
                name="status"
                value={filters.status}
                onChange={handleFilterChange}
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
                <option value="">All Statuses</option>
                <option value="0">Draft</option>
                <option value="1">Submitted</option>
                <option value="2">Under Review</option>
                <option value="3">Preliminary Accepted</option>
                <option value="4">Approved</option>
                <option value="5">Final Accepted</option>
                <option value="6">Rejected</option>
              </select>
            </div>

            {/* Program Filter */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Program
              </label>
              <select
                name="programId"
                value={filters.programId}
                onChange={handleFilterChange}
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
                <option value="">All Programs</option>
                {programs.map((program) => (
                  <option key={program.id} value={program.id}>
                    {program.name}
                  </option>
                ))}
              </select>
            </div>

            {/* Student ID Filter */}
            <div>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Student ID
              </label>
              <input
                type="number"
                name="studentId"
                value={filters.studentId}
                onChange={handleFilterChange}
                placeholder="Enter student ID"
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

          {/* Filter Actions */}
          <div style={{ display: 'flex', gap: '1rem' }}>
            <button
              onClick={handleApplyFilters}
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}
            >
              Apply Filters
            </button>
            <button
              onClick={handleResetFilters}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}
            >
              Reset
            </button>
          </div>
        </div>

        {/* Applications Table */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Applications ({applications.length})
            </h2>
          </div>

          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading applications...</p>
            </div>
          ) : applications.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📄</div>
              <p>No applications found matching the selected filters.</p>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>ID</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Student</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Email</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Program</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Status</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Applied Date</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Grade Requirements</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {applications.map((app) => (
                    <tr
                      key={app.id}
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
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>#{app.id}</td>
                      <td style={{ padding: '0.75rem', fontWeight: '500', color: '#1f2937' }}>{app.studentName}</td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{app.studentEmail}</td>
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937' }}>{app.programName}</div>
                        <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>{app.scientificArea}</div>
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <span style={{
                          display: 'inline-block',
                          padding: '0.25rem 0.75rem',
                          borderRadius: '9999px',
                          backgroundColor: getStatusColor(app.applicationStatus) + '20',
                          color: getStatusColor(app.applicationStatus),
                          fontWeight: '500',
                          fontSize: '0.75rem',
                        }}>
                          {getStatusLabel(app.applicationStatus)}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(app.applicationDate)}
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <span style={{
                          display: 'inline-block',
                          padding: '0.25rem 0.75rem',
                          borderRadius: '0.5rem',
                          backgroundColor: app.meetsGradeRequirements ? '#dcfce7' : '#fee2e2',
                          color: app.meetsGradeRequirements ? '#166534' : '#991b1b',
                          fontWeight: '500',
                          fontSize: '0.75rem',
                        }}>
                          {app.meetsGradeRequirements ? '✓ Meets' : '✗ Does Not Meet'}
                        </span>
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <Link
                          to={`/admin/applications/${app.id}/review`}
                          style={{
                            backgroundColor: '#0d9488',
                            color: 'white',
                            padding: '0.5rem 1rem',
                            borderRadius: '0.5rem',
                            textDecoration: 'none',
                            fontWeight: '500',
                            fontSize: '0.875rem',
                            display: 'inline-block',
                          }}
                        >
                          Review
                        </Link>
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
