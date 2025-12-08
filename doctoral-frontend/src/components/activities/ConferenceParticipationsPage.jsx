import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import ConferenceDetailModal from './ConferenceDetailModal';

export default function ConferenceParticipationsPage() {
  const [conferences, setConferences] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    conferenceName: '',
    date: '',
    role: '',
    isInternational: false,
  });
  const [uploadingDocId, setUploadingDocId] = useState(null);
  const [fileByConference, setFileByConference] = useState({});
  const [studentId, setStudentId] = useState(null);
  const [selectedConferenceForView, setSelectedConferenceForView] = useState(null);

  // Load student profile to get studentId
  useEffect(() => {
    const fetchStudentProfile = async () => {
      try {
        const profile = await studentService.getStudentProfile();
        setStudentId(profile.studentId);
      } catch (err) {
        console.error('Failed to fetch student profile:', err);
        setError('Failed to load student profile. Please log in again.');
      }
    };
    fetchStudentProfile();
  }, []);

  // Load conferences
  useEffect(() => {
    if (!studentId) return;
    fetchConferences();
  }, [studentId]);

  const fetchConferences = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getMyConferences();
      setConferences(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(`Failed to load conferences: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };

  // Add conference – JSON body (fixes 415)
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!studentId) {
      setError('Student ID not found. Please log in again.');
      return;
    }

    try {
      setError('');
      const payload = {
        studentId: studentId,
        conferenceName: formData.conferenceName,
        date: formData.date,
        role: formData.role,
        isInternational: formData.isInternational,
      };

      const result = await studentService.addConferenceParticipation(payload);
      setConferences([
        {
          id: result.id,
          studentId: result.studentId,
          conferenceName: result.conferenceName,
          date: formData.date,
          role: formData.role,
          isInternational: formData.isInternational,
          document: null,
        },
        ...conferences,
      ]);

      setShowForm(false);
      setFormData({ conferenceName: '', date: '', role: '', isInternational: false });
    } catch (err) {
      setError(`Failed to add conference: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    }
  };

  // Handle file selection
  const handleFileChange = (conferenceId, file) => {
    setFileByConference((prev) => ({
      ...prev,
      [conferenceId]: file,
    }));
  };

  // Upload proof document (separate endpoint, multipart/form-data)
  const handleFileUpload = async (conferenceId) => {
    const file = fileByConference[conferenceId];
    if (!file) {
      alert('Please select a file first.');
      return;
    }

    try {
      setUploadingDocId(conferenceId);
      await studentService.uploadConferenceDocument(conferenceId, file, file.name, 3); // 3 = ConferenceProof
      await fetchConferences();
      setFileByConference((prev) => {
        const copy = { ...prev };
        delete copy[conferenceId];
        return copy;
      });
    } catch (err) {
      alert(`Upload failed: ${err.response?.data?.message || err.message}`);
    } finally {
      setUploadingDocId(null);
    }
  };

  const handleDelete = async (conferenceId) => {
    if (!window.confirm('Are you sure you want to delete this conference participation?')) return;
    try {
      await studentService.deleteConference(conferenceId);
      setConferences(conferences.filter((c) => c.id !== conferenceId));
    } catch (err) {
      alert(`Delete failed: ${err.response?.data?.message || err.message}`);
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
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Conference Participations
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              Track your conference presentations and attendances
            </p>
          </div>
          <div style={{ display: 'flex', gap: '1rem' }}>
            <button
              onClick={() => setShowForm(!showForm)}
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
                fontSize: '0.95rem',
              }}
            >
              {showForm ? 'Cancel' : '+ Add Conference'}
            </button>
            <Link
              to="/activities"
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
              ← Back to Activities
            </Link>
          </div>
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

        {/* Add form */}
        {showForm && (
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
              Add New Conference Participation
            </h2>
            <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
              <div>
                <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                  Conference Name *
                </label>
                <input
                  type="text"
                  value={formData.conferenceName}
                  onChange={(e) => setFormData({ ...formData, conferenceName: e.target.value })}
                  required
                  placeholder="e.g., International Conference on AI"
                  style={{
                    width: '100%',
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    boxSizing: 'border-box',
                  }}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div>
                  <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                    Date *
                  </label>
                  <input
                    type="date"
                    value={formData.date}
                    onChange={(e) => setFormData({ ...formData, date: e.target.value })}
                    required
                    style={{
                      width: '100%',
                      padding: '0.75rem',
                      border: '1px solid #d1d5db',
                      borderRadius: '0.5rem',
                      fontSize: '1rem',
                      boxSizing: 'border-box',
                    }}
                  />
                </div>
                <div>
                  <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                    Role *
                  </label>
                  <input
                    type="text"
                    value={formData.role}
                    onChange={(e) => setFormData({ ...formData, role: e.target.value })}
                    placeholder="e.g., Presenter, Attendee"
                    required
                    style={{
                      width: '100%',
                      padding: '0.75rem',
                      border: '1px solid #d1d5db',
                      borderRadius: '0.5rem',
                      fontSize: '1rem',
                      boxSizing: 'border-box',
                    }}
                  />
                </div>
              </div>

              <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: '500', color: '#374151' }}>
                <input
                  type="checkbox"
                  checked={formData.isInternational}
                  onChange={(e) => setFormData({ ...formData, isInternational: e.target.checked })}
                />
                International Conference
              </label>

              <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}>
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  style={{
                    backgroundColor: '#e5e7eb',
                    color: '#1f2937',
                    padding: '0.75rem 1.5rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: 'pointer',
                    fontWeight: '500',
                  }}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={!studentId}
                  style={{
                    background: studentId ? 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)' : '#6b7280',
                    color: 'white',
                    padding: '0.75rem 1.5rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: studentId ? 'pointer' : 'not-allowed',
                    fontWeight: '500',
                  }}
                >
                  Add Conference
                </button>
              </div>
            </form>
          </div>
        )}

        {/* Loading */}
        {loading && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
            <p>Loading your conferences...</p>
          </div>
        )}

        {/* Empty State - Loading student */}
        {!loading && !studentId && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>👤</div>
            <p>Loading student profile...</p>
          </div>
        )}

        {/* Empty State - No conferences */}
        {!loading && studentId && conferences.length === 0 && !showForm && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🎤</div>
            <p style={{ marginBottom: '1rem' }}>No conference participations yet.</p>
            <button
              onClick={() => setShowForm(true)}
              style={{
                background: 'linear-gradient(90deg, #f97316 0%, #ea580c 100%)',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
              }}
            >
              Add your first conference
            </button>
          </div>
        )}

        {/* Conferences List Table */}
        {!loading && studentId && conferences.length > 0 && (
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
                  Your Conference Participations ({conferences.length})
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
                      <td style={{ padding: '1rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937' }}>{conf.conferenceName}</div>
                        <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>ID: {conf.id}</div>
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(conf.date)}
                      </td>
                      <td style={{ padding: '1rem', color: '#374151', fontWeight: '500', fontSize: '0.875rem' }}>
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
                      <td style={{ padding: '1rem' }}>
                        {conf.document ? (
                          <span style={{ color: '#10b981', fontWeight: '500', fontSize: '0.875rem' }}>
                            📎 {conf.document.fileName}
                          </span>
                        ) : (
                          <div style={{ display: 'flex', gap: '0.5rem', flexDirection: 'column' }}>
                            <input
                              type="file"
                              onChange={(e) => {
                                if (e.target.files[0]) {
                                  handleFileChange(conf.id, e.target.files[0]);
                                }
                              }}
                              style={{ fontSize: '0.75rem' }}
                            />
                            {fileByConference[conf.id] && (
                              <button
                                onClick={() => handleFileUpload(conf.id)}
                                disabled={uploadingDocId === conf.id}
                                style={{
                                  backgroundColor: uploadingDocId === conf.id ? '#6b7280' : '#f97316',
                                  color: 'white',
                                  padding: '0.5rem 1rem',
                                  borderRadius: '0.5rem',
                                  border: 'none',
                                  cursor: uploadingDocId === conf.id ? 'not-allowed' : 'pointer',
                                  fontSize: '0.875rem',
                                  fontWeight: '500',
                                }}
                              >
                                {uploadingDocId === conf.id ? 'Uploading...' : 'Upload'}
                              </button>
                            )}
                          </div>
                        )}
                      </td>
                      <td style={{ padding: '1rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
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
                          <button
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
                            Edit
                          </button>
                          <button
                            onClick={() => handleDelete(conf.id)}
                            style={{
                              backgroundColor: '#ef4444',
                              color: 'white',
                              padding: '0.5rem 0.75rem',
                              borderRadius: '0.5rem',
                              border: 'none',
                              cursor: 'pointer',
                              fontSize: '0.75rem',
                              fontWeight: '500',
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
