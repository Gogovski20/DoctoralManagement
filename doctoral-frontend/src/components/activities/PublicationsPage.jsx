import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';
import PublicationDetailModal from './PublicationDetailModal';
import EditPublicationModal from '../student/EditPublicationModal';

export default function PublicationsPage() {
  const [publications, setPublications] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState({
    title: '',
    journal: '',
    publishedOn: '',
    doi: '',
    isIndexedInScopus: false,
    isIndexedInThomsonReuters: false,
  });
  const [uploadingDocId, setUploadingDocId] = useState(null);
  const [fileByPublication, setFileByPublication] = useState({});
  const [studentId, setStudentId] = useState(null);
  const [selectedPublicationForView, setSelectedPublicationForView] = useState(null);
  const [selectedPublicationForEdit, setSelectedPublicationForEdit] = useState(null);


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


  // Load publications
  useEffect(() => {
    if (!studentId) return;
    fetchPublications();
  }, [studentId]);


  const fetchPublications = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getMyPublications();
      setPublications(Array.isArray(data) ? data : []);
    } catch (err) {
      setError(`Failed to load publications: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    } finally {
      setLoading(false);
    }
  };


  // Add publication
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
        title: formData.title,
        journal: formData.journal,
        publishedOn: formData.publishedOn,
        doi: formData.doi,
        isIndexedInScopus: formData.isIndexedInScopus,
        isIndexedInThomsonReuters: formData.isIndexedInThomsonReuters,
      };


      const result = await studentService.addPublication(payload);
      setPublications([
        {
          id: result.id,
          studentId: result.studentId,
          title: result.title,
          journal: formData.journal,
          publishedOn: formData.publishedOn,
          doi: formData.doi,
          isIndexedInScopus: formData.isIndexedInScopus,
          isIndexedInThomsonReuters: formData.isIndexedInThomsonReuters,
          document: null,
        },
        ...publications,
      ]);


      setShowForm(false);
      setFormData({
        title: '',
        journal: '',
        publishedOn: '',
        doi: '',
        isIndexedInScopus: false,
        isIndexedInThomsonReuters: false,
      });
    } catch (err) {
      setError(`Failed to add publication: ${err.response?.data?.message || err.message || 'Unknown error'}`);
    }
  };


  // Handle file selection
  const handleFileChange = (publicationId, file) => {
    setFileByPublication((prev) => ({
      ...prev,
      [publicationId]: file,
    }));
  };


  // Upload document
  const handleFileUpload = async (publicationId) => {
    const file = fileByPublication[publicationId];
    if (!file) {
      alert('Please select a file first.');
      return;
    }


    try {
      setUploadingDocId(publicationId);
      await studentService.uploadPublicationDocument(publicationId, file, file.name, 1); // 1 = PublicationProof
      await fetchPublications();
      setFileByPublication((prev) => {
        const copy = { ...prev };
        delete copy[publicationId];
        return copy;
      });
    } catch (err) {
      alert(`Upload failed: ${err.response?.data?.message || err.message}`);
    } finally {
      setUploadingDocId(null);
    }
  };


  const handleDelete = async (publicationId) => {
    if (!window.confirm('Are you sure you want to delete this publication?')) return;
    try {
      await studentService.deletePublication(publicationId);
      setPublications(publications.filter((p) => p.id !== publicationId));
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
              Publications
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              Manage your journal articles and research publications
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
              {showForm ? 'Cancel' : '+ Add Publication'}
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
              Add New Publication
            </h2>
            <form onSubmit={handleSubmit} style={{ display: 'grid', gap: '1rem' }}>
              <div>
                <label style={{ display: 'block', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                  Title *
                </label>
                <input
                  type="text"
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  required
                  placeholder="Publication title"
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
                  Journal *
                </label>
                <input
                  type="text"
                  value={formData.journal}
                  onChange={(e) => setFormData({ ...formData, journal: e.target.value })}
                  required
                  placeholder="Journal name"
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
                    Published On *
                  </label>
                  <input
                    type="date"
                    value={formData.publishedOn}
                    onChange={(e) => setFormData({ ...formData, publishedOn: e.target.value })}
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
                    DOI
                  </label>
                  <input
                    type="text"
                    value={formData.doi}
                    onChange={(e) => setFormData({ ...formData, doi: e.target.value })}
                    placeholder="e.g., 10.1234/example"
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


              <div style={{ display: 'flex', gap: '1rem', flexDirection: 'column' }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: '500', color: '#374151' }}>
                  <input
                    type="checkbox"
                    checked={formData.isIndexedInScopus}
                    onChange={(e) => setFormData({ ...formData, isIndexedInScopus: e.target.checked })}
                  />
                  Indexed in Scopus
                </label>
                <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: '500', color: '#374151' }}>
                  <input
                    type="checkbox"
                    checked={formData.isIndexedInThomsonReuters}
                    onChange={(e) => setFormData({ ...formData, isIndexedInThomsonReuters: e.target.checked })}
                  />
                  Indexed in Thomson Reuters
                </label>
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
                  Add Publication
                </button>
              </div>
            </form>
          </div>
        )}


        {/* Loading */}
        {loading && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
            <p>Loading your publications...</p>
          </div>
        )}


        {/* Empty State - Loading student */}
        {!loading && !studentId && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>👤</div>
            <p>Loading student profile...</p>
          </div>
        )}


        {/* Empty State - No publications */}
        {!loading && studentId && publications.length === 0 && !showForm && (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>📚</div>
            <p style={{ marginBottom: '1rem' }}>No publications yet.</p>
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
              Add your first publication
            </button>
          </div>
        )}


        {/* Publications List Table */}
        {!loading && studentId && publications.length > 0 && (
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
                  Your Publications ({publications.length})
                </h2>
                <button
                  onClick={fetchPublications}
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
                      Title
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Journal
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Published
                    </th>
                    <th style={{ textAlign: 'left', padding: '1rem', fontWeight: '600', color: '#374151', fontSize: '0.875rem' }}>
                      Indexing
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
                  {publications.map((pub) => (
                    <tr key={pub.id} style={{ borderBottom: '1px solid #e5e7eb' }}>
                      <td style={{ padding: '1rem' }}>
                        <div style={{ fontWeight: '500', color: '#1f2937', maxWidth: '200px', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                          {pub.title}
                        </div>
                        <div style={{ fontSize: '0.875rem', color: '#6b7280' }}>ID: {pub.id}</div>
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {pub.journal}
                      </td>
                      <td style={{ padding: '1rem', color: '#6b7280', fontSize: '0.875rem' }}>
                        {formatDate(pub.publishedOn)}
                      </td>
                      <td style={{ padding: '1rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem', flexDirection: 'column' }}>
                          <span
                            style={{
                              display: 'inline-block',
                              padding: '0.25rem 0.5rem',
                              borderRadius: '0.25rem',
                              backgroundColor: pub.isIndexedInScopus ? '#10b98120' : '#6b728020',
                              color: pub.isIndexedInScopus ? '#10b981' : '#6b7280',
                              fontSize: '0.7rem',
                              fontWeight: '500',
                              width: 'fit-content',
                            }}
                          >
                            {pub.isIndexedInScopus ? '✓' : '✗'} Scopus
                          </span>
                          <span
                            style={{
                              display: 'inline-block',
                              padding: '0.25rem 0.5rem',
                              borderRadius: '0.25rem',
                              backgroundColor: pub.isIndexedInThomsonReuters ? '#10b98120' : '#6b728020',
                              color: pub.isIndexedInThomsonReuters ? '#10b981' : '#6b7280',
                              fontSize: '0.7rem',
                              fontWeight: '500',
                              width: 'fit-content',
                            }}
                          >
                            {pub.isIndexedInThomsonReuters ? '✓' : '✗'} TR
                          </span>
                        </div>
                      </td>
                      <td style={{ padding: '1rem' }}>
                        {pub.document ? (
                          <span style={{ color: '#10b981', fontWeight: '500', fontSize: '0.875rem' }}>
                            📎 {pub.document.fileName}
                          </span>
                        ) : (
                          <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                            <input
                              type="file"
                              id={`file-${pub.id}`}
                              onChange={(e) => handleFileChange(pub.id, e.target.files[0])}
                              style={{ display: 'none' }}
                            />
                            <label
                              htmlFor={`file-${pub.id}`}
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
                            {fileByPublication[pub.id] && (
                              <button
                                onClick={() => handleFileUpload(pub.id)}
                                disabled={uploadingDocId === pub.id}
                                style={{
                                  backgroundColor: uploadingDocId === pub.id ? '#9ca3af' : '#10b981',
                                  color: 'white',
                                  padding: '0.35rem 0.5rem',
                                  borderRadius: '0.35rem',
                                  border: 'none',
                                  fontSize: '0.7rem',
                                  fontWeight: '500',
                                  cursor: uploadingDocId === pub.id ? 'not-allowed' : 'pointer',
                                }}
                              >
                                {uploadingDocId === pub.id ? 'Uploading...' : 'Upload'}
                              </button>
                            )}
                          </div>
                        )}
                      </td>


                      <td style={{ padding: '1rem' }}>
                        <div style={{ display: 'flex', gap: '0.5rem' }}>
                          <button
                            onClick={() => setSelectedPublicationForView(pub.id)}
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
                            onClick={() => setSelectedPublicationForEdit(pub.id)}
                            disabled={pub.isApproved}
                            style={{
                              backgroundColor: pub.isApproved ? '#9ca3af' : '#f59e0b',
                              color: 'white',
                              padding: '0.5rem 0.75rem',
                              borderRadius: '0.5rem',
                              border: 'none',
                              cursor: pub.isApproved ? 'not-allowed' : 'pointer',
                              fontSize: '0.75rem',
                              fontWeight: '500',
                              opacity: pub.isApproved ? 0.6 : 1,
                            }}
                          >
                            Edit
                          </button>
                          <button
                            onClick={() => handleDelete(pub.id)}
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
        {selectedPublicationForView && (
          <PublicationDetailModal
            publicationId={selectedPublicationForView}
            onClose={() => setSelectedPublicationForView(null)}
          />
        )}

        {/* Edit Modal */}
        {selectedPublicationForEdit && (
          <EditPublicationModal
            publicationId={selectedPublicationForEdit}
            onClose={() => setSelectedPublicationForEdit(null)}
            onSuccess={() => {
              fetchPublications();
              setSelectedPublicationForEdit(null);
            }}
          />
        )}
      </div>
    </div>
  );
}