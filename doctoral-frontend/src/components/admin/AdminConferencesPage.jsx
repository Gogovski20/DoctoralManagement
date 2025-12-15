import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import ConferenceDetailModal from '../activities/ConferenceDetailModal';



export default function AdminConferencesPage() {
  const navigate = useNavigate();
  const [conferences, setConferences] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedConferenceForView, setSelectedConferenceForView] = useState(null);


  useEffect(() => {
    fetchConferences();
  }, []);


  const fetchConferences = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getAllConferences();
      setConferences(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(`Failed to load conferences: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
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


  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1400px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              All Conference Participations
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              View all student conference participation records
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


        {/* Loading */}
        {loading && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
            <p>Loading conferences...</p>
          </div>
        )}


        {/* Empty State */}
        {!loading && conferences.length === 0 && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🎤</div>
            <p>No conference participations found.</p>
          </div>
        )}


        {/* Conferences Table */}
        {!loading && conferences.length > 0 && (
          <div
            style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
              border: '1px solid #e5e7eb',
              overflow: 'hidden',
            }}
          >
            <div style={{ padding: '1.5rem' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
                  Conferences ({conferences.length})
                </h2>
                <button
                  onClick={fetchConferences}
                  style={{
                    backgroundColor: 'transparent',
                    color: '#0d9488',
                    padding: '0.5rem 1rem',
                    borderRadius: '0.5rem',
                    border: '1px solid #0d9488',
                    cursor: 'pointer',
                    fontWeight: '500',
                  }}
                >
                  Refresh
                </button>
              </div>
            </div>


            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse' }}>
                <thead>
                  <tr style={{ backgroundColor: '#f9fafb', borderBottom: '2px solid #e5e7eb' }}>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Student
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Conference
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Date
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Role
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      International
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Document
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {conferences.map((conf) => (
                    <tr key={conf.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                      <td style={{ padding: '1rem', fontWeight: '500', color: '#1f2937' }}>
                        {conf.studentName || 'N/A'}
                      </td>
                      <td style={{ padding: '1rem', color: '#374151' }}>
                        <div style={{ fontWeight: '500' }}>{conf.conferenceName}</div>
                        <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>ID: {conf.id}</div>
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(conf.date)}
                      </td>
                      <td style={{ padding: '1rem', color: '#374151', fontSize: '0.875rem' }}>
                        {conf.role}
                      </td>
                      <td style={{ padding: '1rem' }}>
                        <span
                          style={{
                            display: 'inline-block',
                            padding: '0.25rem 0.75rem',
                            borderRadius: '9999px',
                            backgroundColor: conf.isInternational ? '#10b98120' : '#6b728020',
                            color: conf.isInternational ? '#10b981' : '#6b7280',
                            fontSize: '0.75rem',
                            fontWeight: '500',
                          }}
                        >
                          {conf.isInternational ? '🌍 Yes' : 'No'}
                        </span>
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {conf.document ? (
                          <span
                            style={{
                              backgroundColor: '#f0f9ff',
                              color: '#0369a1',
                              padding: '0.375rem 0.75rem',
                              borderRadius: '0.375rem',
                              fontWeight: '500',
                              border: '1px solid #bae6fd',
                            }}
                          >
                            📄 {conf.document.fileName}
                          </span>
                        ) : (
                          <span style={{ color: '#9ca3af', fontSize: '0.875rem' }}>No file</span>
                        )}
                      </td>
                      <td style={{ padding: '1rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
                          <button
                            onClick={() => setSelectedConferenceForView(conf.id)}
                            style={{
                              backgroundColor: '#3b82f6',
                              color: 'white',
                              padding: '0.5rem 0.75rem',
                              borderRadius: '0.5rem',
                              border: 'none',
                              cursor: 'pointer',
                              fontSize: '0.75rem',
                              fontWeight: '500',
                            }}
                          >
                            View
                          </button>
                          {!conf.isApproved && (
                            <button
                              onClick={() => navigate(`/admin/conferences/${conf.id}/review`)}
                              style={{
                                backgroundColor: '#f59e0b',
                                color: 'white',
                                padding: '0.5rem 0.75rem',
                                borderRadius: '0.5rem',
                                border: 'none',
                                cursor: 'pointer',
                                fontSize: '0.75rem',
                                fontWeight: '500',
                              }}
                            >
                              Review
                            </button>
                          )}
                          {conf.isApproved && (
                            <span
                              style={{
                                backgroundColor: '#10b98120',
                                color: '#10b981',
                                padding: '0.5rem 0.75rem',
                                borderRadius: '0.5rem',
                                fontSize: '0.75rem',
                                fontWeight: '500',
                              }}
                            >
                              ✅ Approved
                            </span>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}


        {/* Detail Modal */}
        {selectedConferenceForView && (
          <ConferenceDetailModal
            conferenceId={selectedConferenceForView}
            onClose={() => setSelectedConferenceForView(null)}
          />
        )}
      </div>
    </div>
  );
}