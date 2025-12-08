import React, { useState, useEffect, useContext } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import { AuthContext } from '../../context/AuthContext';

export default function StudentDashboard() {
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchApplications();
  }, []);

  const fetchApplications = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getApplications();
      console.log('Applications data:', data); // Debug log
      setApplications(Array.isArray(data) ? data : []);
    } catch (err) {
      console.error('Failed to fetch applications:', err);
      setError(`Failed to load applications: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };

  const getStatusColor = (status) => {
    if (!status) return '#6b7280';
    
    const statusStr = status.toString().toLowerCase();
    switch (statusStr) {
      case 'draft':
        return '#6b7280';
      case 'submitted':
      case 'submitted_for_review':
        return '#3b82f6';
      case 'approved':
      case 'accepted':
      case 'final_accepted':
        return '#10b981';
      case 'rejected':
      case 'declined':
        return '#ef4444';
      case 'under_review':
      case 'in_review':
        return '#f59e0b';
      default:
        return '#6b7280';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return 'Draft';
    
    const statusStr = status.toString();
    // Convert "Submitted" or enum string to readable format
    return statusStr
      .replace(/_/g, ' ')
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

  const { user } = useContext(AuthContext); 
  const studentId = user?.studentId;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Student Dashboard
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Manage your doctoral journey
          </p>
        </div>

        {/* Application Statistics */}
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
          gap: '1rem',
          marginBottom: '2rem',
        }}>
          <StatCard 
            title="Total Applications"
            value={applications.length}
            icon="📄"
            color="#3b82f6"
          />
          <StatCard 
            title="Submitted"
            value={applications.filter(app => app.applicationStatus === 'Submitted' || app.applicationStatus === 1).length}
            icon="📤"
            color="#10b981"
          />
          <StatCard 
            title="In Draft"
            value={applications.filter(app => app.applicationStatus === 'Draft' || app.applicationStatus === 0).length}
            icon="✏️"
            color="#6b7280"
          />
          <StatCard 
            title="Under Review"
            value={applications.filter(app => 
              app.applicationStatus === 'UnderReview' || 
              app.applicationStatus === 'InReview' || 
              app.applicationStatus === 2
            ).length}
            icon="🔍"
            color="#f59e0b"
          />
        </div>

        {/* Quick Actions Grid */}
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
          gap: '1rem',
          marginBottom: '2rem',
        }}>
          <ActionCard 
            to="/applications/new"
            icon="📝" 
            title="New Application"
            gradient="linear-gradient(135deg, #3b82f6 0%, #1e40af 100%)"
          />
          <ActionCard 
            to="/courses"
            icon="📚" 
            title="Courses"
            gradient="linear-gradient(135deg, #10b981 0%, #059669 100%)"
          />
          <ActionCard 
            to="/doctoral-project"
            icon="🔬" 
            title="Doctoral Projects"
            gradient="linear-gradient(135deg, #8b5cf6 0%, #6d28d9 100%)"
          />
          <ActionCard 
            to="/activities"
            icon="🌍" 
            title="Activities"
            gradient="linear-gradient(135deg, #f97316 0%, #ea580c 100%)"
          />
          <ActionCard 
            to="/ects-tracking"  // ✓ No parameter needed
            icon="📊" 
            title="ECTS Tracking"
            gradient="linear-gradient(135deg, #0d9488 0%, #059669 100%)"
          />


        </div>

        {/* Applications Section */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Your Applications
            </h2>
            <button
              onClick={fetchApplications}
              style={{
                backgroundColor: 'transparent',
                color: '#0d9488',
                padding: '0.5rem 1rem',
                borderRadius: '0.5rem',
                border: '1px solid #0d9488',
                cursor: 'pointer',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}
            >
              Refresh
            </button>
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

          {loading ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
              <p>Loading your applications...</p>
            </div>
          ) : applications.length === 0 ? (
            <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
              <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📄</div>
              <p style={{ marginBottom: '1rem' }}>No applications yet.</p>
              <Link to="/applications/new" style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                textDecoration: 'none',
                fontWeight: '500',
                display: 'inline-block',
              }}>
                Create your first application
              </Link>
            </div>
          ) : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>ID</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Program</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Faculty</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Status</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Applied Date</th>
                    <th style={{ textAlign: 'left', padding: '0.75rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>Decision Date</th>
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
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937' }}>{app.programName}</div>
                        <div style={{ fontSize: '0.75rem', color: '#6b7280' }}>Program ID: {app.doctoralProgramId}</div>
                      </td>
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>{app.faculty}</td>
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
                      <td style={{ padding: '0.75rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {app.decisionDate ? formatDate(app.decisionDate) : 'Pending'}
                      </td>
                      <td style={{ padding: '0.75rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          <Link to={`/applications/${app.id}`} style={{
                            backgroundColor: '#0d9488',
                            color: 'white',
                            padding: '0.5rem 1rem',
                            borderRadius: '0.5rem',
                            textDecoration: 'none',
                            fontWeight: '500',
                            fontSize: '0.875rem',
                          }}>
                            View
                          </Link>
                          {(app.applicationStatus === 'Draft' || app.applicationStatus === 0) && (
                            <Link to={`/applications/${app.id}/upload`} style={{
                              backgroundColor: '#3b82f6',
                              color: 'white',
                              padding: '0.5rem 1rem',
                              borderRadius: '0.5rem',
                              textDecoration: 'none',
                              fontWeight: '500',
                              fontSize: '0.875rem',
                            }}>
                              Upload Docs
                            </Link>
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
    </div>
  );
}

function ActionCard({ to, icon, title, gradient }) {
  return (
    <Link to={to} style={{ textDecoration: 'none' }}>
      <div style={{
        background: gradient,
        color: 'white',
        padding: '1.5rem',
        borderRadius: '0.75rem',
        boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
        cursor: 'pointer',
        transition: 'all 0.2s',
        height: '100%',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.boxShadow = '0 20px 25px -5px rgba(0, 0, 0, 0.2)';
        e.currentTarget.style.transform = 'translateY(-2px)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1)';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
      >
        <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>{icon}</div>
        <h3 style={{ fontWeight: '600', margin: 0, fontSize: '1rem' }}>{title}</h3>
      </div>
    </Link>
  );
}

function StatCard({ title, value, icon, color }) {
  return (
    <div style={{
      backgroundColor: 'white',
      borderRadius: '0.75rem',
      padding: '1.5rem',
      boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
      border: `1px solid ${color}20`,
    }}>
      <div style={{ display: 'flex', alignItems: 'center', marginBottom: '0.75rem' }}>
        <div style={{
          backgroundColor: `${color}20`,
          color: color,
          width: '3rem',
          height: '3rem',
          borderRadius: '50%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          marginRight: '1rem',
          fontSize: '1.5rem',
        }}>
          {icon}
        </div>
        <div style={{ fontSize: '2rem', fontWeight: 'bold', color: color }}>
          {value}
        </div>
      </div>
      <div style={{ color: '#6b7280', fontSize: '0.875rem', fontWeight: '500' }}>
        {title}
      </div>
    </div>
  );
}