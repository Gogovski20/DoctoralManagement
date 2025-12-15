import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import MobilityDetailModal from './MobilityDetailModal';
import EditMobilityModal from '../student/EditMobilityModal';

export default function MobilitiesPage() {
  const [mobilities, setMobilities] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    institution: '',
    country: '',
    startDate: '',
    endDate: '',
  });
  const [uploadingDocId, setUploadingDocId] = useState(null);
  const [fileByMobility, setFileByMobility] = useState({});
  const [studentId, setStudentId] = useState(null);
  const [selectedMobilityForView, setSelectedMobilityForView] = useState(null);
  const [selectedMobilityForEdit, setSelectedMobilityForEdit] = useState(null);


  // Load student profile
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


  // Load mobilities
  useEffect(() => {
    if (!studentId) return;
    fetchMobilities();
  }, [studentId]);


  const fetchMobilities = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getMyMobilities();
      setMobilities(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(`Failed to load mobilities: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };


  // Add mobility
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!studentId) {
      setError('Student ID not found. Please log in again.');
      return;
    }


    // Validate dates
    if (new Date(formData.endDate) <= new Date(formData.startDate)) {
      setError('End date must be after start date.');
      return;
    }


    try {
      setError('');
      const payload = {
        studentId: studentId,
        institution: formData.institution,
        country: formData.country,
        startDate: formData.startDate,
        endDate: formData.endDate,
      };


      const result = await studentService.addMobility(payload);
      setMobilities([
        {
          id: result.id,
          studentId: result.studentId,
          institution: formData.institution,
          country: formData.country,
          startDate: formData.startDate,
          endDate: formData.endDate,
          document: null,
        },
        ...mobilities,
      ]);


      setShowForm(false);
      setFormData({
        institution: '',
        country: '',
        startDate: '',
        endDate: '',
      });
    } catch (err) {
      setError(`Failed to add mobility: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    }
  };


  // Handle file selection
  const handleFileChange = (mobilityId, file) => {
    setFileByMobility((prev) => ({
      ...prev,
      [mobilityId]: file,
    }));
  };


  // Upload document
  const handleFileUpload = async (mobilityId) => {
    const file = fileByMobility[mobilityId];
    if (!file) {
      alert('Please select a file first.');
      return;
    }


    try {
      setUploadingDocId(mobilityId);
      await studentService.uploadMobilityDocument(mobilityId, file, file.name, 2); // 2 = MobilityProof
      await fetchMobilities();
      setFileByMobility((prev) => {
        const copy = { ...prev };
        delete copy[mobilityId];
        return copy;
      });
    } catch (err) {
      alert(`Upload failed: ${err.response?.data?.message || err.message}`);
    } finally {
      setUploadingDocId(null);
    }
  };


  const handleDelete = async (mobilityId) => {
    if (!window.confirm('Are you sure you want to delete this mobility record?')) return;
    try {
      await studentService.deleteMobility(mobilityId);
      setMobilities(mobilities.filter((m) => m.id !== mobilityId));
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


  const calculateDuration = (startDate, endDate) => {
    if (!startDate || !endDate) return 'N/A';
    try {
      const start = new Date(startDate);
      const end = new Date(endDate);
      const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24));
      return `${days} days`;
    } catch {
      return 'N/A';
    }
  };


  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Mobilities
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              Track your research stays and international exchanges
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
              {showForm ? 'Cancel' : '+ Add Mobility'}
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
              Add New Mobility
            </h2>
            <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div>
                  <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                    Institution *
                  </label>
                  <input
                    type="text"
                    value={formData.institution}
                    onChange={(e) => setFormData({ ...formData, institution: e.target.value })}
                    required
                    placeholder="e.g., University of Milan"
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
                    Country *
                  </label>
                  <input
                    type="text"
                    value={formData.country}
                    onChange={(e) => setFormData({ ...formData, country: e.target.value })}
                    required
                    placeholder="e.g., Italy"
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


              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem' }}>
                <div>
                  <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                    Start Date *
                  </label>
                  <input
                    type="date"
                    value={formData.startDate}
                    onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
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
                    End Date *
                  </label>
                  <input
                    type="date"
                    value={formData.endDate}
                    onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
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
                  Add Mobility
                </button>
              </div>
            </form>
          </div>
        )}


        {/* Loading */}
        {loading && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
            <p>Loading your mobilities...</p>
          </div>
        )}


        {/* Empty State - Loading student */}
        {!loading && !studentId && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>👤</div>
            <p>Loading student profile...</p>
          </div>
        )}


        {/* Empty State - No mobilities */}
        {!loading && studentId && mobilities.length === 0 && !showForm && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>✈️</div>
            <p style={{ marginBottom: '1rem' }}>No mobilities yet.</p>
            <button
              onClick={() => setShowForm(true)}
              style={{
                background: 'linear-gradient(90deg, #3b82f6 0%, #1e40af 100%)',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '500',
              }}
            >
              Add your first mobility
            </button>
          </div>
        )}


        {/* Mobilities List Table */}
        {!loading && studentId && mobilities.length > 0 && (
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
                  Your Mobilities ({mobilities.length})
                </h2>
                <button
                  onClick={fetchMobilities}
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
                      Institution
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Country
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Duration
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Days
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
                  {mobilities.map((mobility) => (
                    <tr key={mobility.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                      <td style={{ padding: '1rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937' }}>
                          {mobility.institution}
                        </div>
                        <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>ID: {mobility.id}</div>
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        🌍 {mobility.country}
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(mobility.startDate)} - {formatDate(mobility.endDate)}
                      </td>
                      <td style={{ padding: '1rem', color: '#1f2937', fontWeight: '500', fontSize: '0.875rem' }}>
                        {calculateDuration(mobility.startDate, mobility.endDate)}
                      </td>
                      <td style={{ padding: '1rem' }}>
                        {mobility.document ? (
                          <span style={{ color: '#10b981', fontWeight: '500', fontSize: '0.875rem' }}>
                            📎 {mobility.document.fileName}
                          </span>
                        ) : (
                          <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                            <input
                              type="file"
                              id={`file-${mobility.id}`}
                              onChange={(e) => handleFileChange(mobility.id, e.target.files[0])}
                              style={{ display: 'none' }}
                            />
                            <label
                              htmlFor={`file-${mobility.id}`}
                              style={{
                                backgroundColor: '#e5e7eb',
                                color: '#374151',
                                padding: '0.35rem 0.5rem',
                                borderRadius: '0.35rem',
                                fontSize: '0.7rem',
                                fontWeight: '500',
                                cursor: 'pointer',
                              }}
                            >
                              Choose
                            </label>
                            {fileByMobility[mobility.id] && (
                              <button
                                onClick={() => handleFileUpload(mobility.id)}
                                disabled={uploadingDocId === mobility.id}
                                style={{
                                  backgroundColor: uploadingDocId === mobility.id ? '#9ca3af' : '#10b981',
                                  color: 'white',
                                  padding: '0.35rem 0.5rem',
                                  borderRadius: '0.35rem',
                                  border: 'none',
                                  fontSize: '0.7rem',
                                  fontWeight: '500',
                                  cursor: uploadingDocId === mobility.id ? 'not-allowed' : 'pointer',
                                }}
                              >
                                {uploadingDocId === mobility.id ? 'Uploading...' : 'Upload'}
                              </button>
                            )}
                          </div>
                        )}
                      </td>


                      <td style={{ padding: '1rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          <button
                            onClick={() => setSelectedMobilityForView(mobility.id)}
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
                            onClick={() => setSelectedMobilityForEdit(mobility.id)}
                            disabled={mobility.isApproved}
                            style={{
                              backgroundColor: mobility.isApproved ? '#9ca3af' : '#f59e0b',
                              color: 'white',
                              padding: '0.5rem 0.75rem',
                              borderRadius: '0.5rem',
                              border: 'none',
                              cursor: mobility.isApproved ? 'not-allowed' : 'pointer',
                              fontSize: '0.75rem',
                              fontWeight: '500',
                              opacity: mobility.isApproved ? 0.6 : 1,
                            }}
                          >
                            Edit
                          </button>
                          <button
                            onClick={() => handleDelete(mobility.id)}
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
        {selectedMobilityForView && (
          <MobilityDetailModal
            mobilityId={selectedMobilityForView}
            onClose={() => setSelectedMobilityForView(null)}
          />
        )}

        {/* Edit Modal */}
        {selectedMobilityForEdit && (
          <EditMobilityModal
            mobilityId={selectedMobilityForEdit}
            onClose={() => setSelectedMobilityForEdit(null)}
            onSuccess={() => {
              fetchMobilities();
              setSelectedMobilityForEdit(null);
            }}
          />
        )}
      </div>
    </div>
  );
}