import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminReviewConferencePage() {
  const { conferenceId } = useParams();
  const navigate = useNavigate();
  const [conference, setConference] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [formData, setFormData] = useState({
    conferenceId: parseInt(conferenceId),
    isApproved: true,
    reviewComments: '',
    ectsAwarded: 0,
  });

  useEffect(() => {
    fetchConferenceDetails();
  }, [conferenceId]);

  const fetchConferenceDetails = async () => {
    try {
      setLoading(true);
      setError('');
      const data = await studentService.getConferenceById(conferenceId);
      setConference(data);
      setFormData((prev) => ({
        ...prev,
        ectsAwarded: Math.min(18, data.ectsAwarded || 0),
      }));
    } catch (err) {
      setError(`Failed to load conference details: ${err.response?.data?.message || err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!formData.reviewComments.trim()) {
      setError('Please provide review comments.');
      return;
    }

    try {
      setSubmitting(true);
      setError('');
      await studentService.reviewConference(conferenceId, formData);
      navigate('/admin/conferences', {
        state: { message: `Conference ${formData.isApproved ? 'approved' : 'rejected'} successfully!` },
      });
    } catch (err) {
      setError(`Failed to submit review: ${err.response?.data?.message || err.message}`);
    } finally {
      setSubmitting(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return 'Invalid Date';
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '1000px', margin: '0 auto', textAlign: 'center', paddingTop: '3rem' }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>⏳</div>
          <p style={{ color: '#6b7280' }}>Loading conference details...</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1000px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link
            to="/admin/conferences"
            style={{
              color: '#0d9488',
              textDecoration: 'none',
              fontSize: '0.875rem',
              fontWeight: '500',
              marginBottom: '1rem',
              display: 'inline-block',
            }}
          >
            ← Back to Conferences
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Review Conference Participation
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Evaluate and approve or reject this conference participation
          </p>
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
              marginBottom: '1.5rem',
            }}
          >
            {error}
          </div>
        )}

        {!conference ? (
          <div style={{ textAlign: 'center', color: '#6b7280', padding: '3rem' }}>
            <p>Conference not found.</p>
          </div>
        ) : (
          <div
            style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
              border: '1px solid #e5e7eb',
              padding: '2rem',
            }}
          >
            {/* Conference Details */}
            <div style={{ marginBottom: '2rem' }}>
              <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1.5rem' }}>
                Conference Information
              </h2>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1.5rem', marginBottom: '2rem' }}>
                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    Student
                  </label>
                  <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                    {conference.studentName}
                  </p>
                </div>

                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    Conference Name
                  </label>
                  <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                    {conference.conferenceName}
                  </p>
                </div>

                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    Date
                  </label>
                  <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                    {formatDate(conference.publishedOn)}
                  </p>
                </div>

                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    Role
                  </label>
                  <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                    {conference.role}
                  </p>
                </div>

                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    International
                  </label>
                  <p style={{ fontSize: '1rem', color: '#1f2937', margin: 0 }}>
                    {conference.isInternational ? '🌍 Yes' : 'No'}
                  </p>
                </div>

                <div>
                  <label style={{ display: 'block', fontWeight: '600', color: '#6b7280', fontSize: '0.875rem', marginBottom: '0.5rem' }}>
                    Document
                  </label>
                  {conference.document ? (
                    <p style={{ fontSize: '0.875rem', color: '#10b981', margin: 0 }}>
                      📎 {conference.document.fileName}
                    </p>
                  ) : (
                    <p style={{ fontSize: '0.875rem', color: '#ef4444', margin: 0 }}>
                      ⚠️ No document
                    </p>
                  )}
                </div>
              </div>

              <div style={{ borderTop: '1px solid #e5e7eb', paddingTop: '1.5rem' }} />
            </div>

            {/* Review Form */}
            <form onSubmit={handleSubmit}>
              <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1.5rem' }}>
                Review Decision
              </h2>

              {/* Approval Status */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{ display: 'block', fontWeight: '600', color: '#374151', marginBottom: '1rem' }}>
                  Decision *
                </label>
                <div style={{ display: 'flex', gap: '2rem' }}>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input
                      type="radio"
                      name="isApproved"
                      value="approve"
                      checked={formData.isApproved}
                      onChange={(e) => {
                        setFormData({ ...formData, isApproved: true });
                      }}
                      style={{ cursor: 'pointer' }}
                    />
                    <span style={{ fontWeight: '500', color: '#1f2937' }}>✅ Approve</span>
                  </label>
                  <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', cursor: 'pointer' }}>
                    <input
                      type="radio"
                      name="isApproved"
                      value="reject"
                      checked={!formData.isApproved}
                      onChange={(e) => {
                        setFormData({ ...formData, isApproved: false });
                      }}
                      style={{ cursor: 'pointer' }}
                    />
                    <span style={{ fontWeight: '500', color: '#1f2937' }}>❌ Reject</span>
                  </label>
                </div>
              </div>

              {/* ECTS Awarded (only if approved) */}
              {formData.isApproved && (
                <div style={{ marginBottom: '1.5rem' }}>
                  <label style={{ display: 'block', fontWeight: '600', color: '#374151', marginBottom: '0.5rem' }}>
                    ECTS Points to Award (0-18) *
                  </label>
                  <input
                    type="number"
                    min="0"
                    max="18"
                    value={formData.ectsAwarded}
                    onChange={(e) => {
                      const val = Math.min(18, Math.max(0, parseInt(e.target.value) || 0));
                      setFormData({ ...formData, ectsAwarded: val });
                    }}
                    required={formData.isApproved}
                    style={{
                      width: '100%',
                      maxWidth: '150px',
                      padding: '0.75rem',
                      border: '1px solid #d1d5db',
                      borderRadius: '0.5rem',
                      fontSize: '1rem',
                      boxSizing: 'border-box',
                    }}
                  />
                  <p style={{ fontSize: '0.875rem', color: '#6b7280', marginTop: '0.5rem' }}>
                    Conference participation activities are capped at 18 ECTS maximum.
                  </p>
                </div>
              )}

              {/* Review Comments */}
              <div style={{ marginBottom: '1.5rem' }}>
                <label style={{ display: 'block', fontWeight: '600', color: '#374151', marginBottom: '0.5rem' }}>
                  Review Comments *
                </label>
                <textarea
                  value={formData.reviewComments}
                  onChange={(e) => setFormData({ ...formData, reviewComments: e.target.value })}
                  maxLength={1000}
                  required
                  placeholder="Provide your review comments (max 1000 characters)"
                  style={{
                    width: '100%',
                    minHeight: '150px',
                    padding: '0.75rem',
                    border: '1px solid #d1d5db',
                    borderRadius: '0.5rem',
                    fontSize: '1rem',
                    fontFamily: 'inherit',
                    boxSizing: 'border-box',
                    resize: 'vertical',
                  }}
                />
                <p style={{ fontSize: '0.875rem', color: '#6b7280', marginTop: '0.5rem' }}>
                  {formData.reviewComments.length}/1000 characters
                </p>
              </div>

              {/* Action Buttons */}
              <div style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end', marginTop: '2rem' }}>
                <Link
                  to="/admin/conferences"
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
                  Cancel
                </Link>
                <button
                  type="submit"
                  disabled={submitting}
                  style={{
                    background: submitting ? '#6b7280' : 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
                    color: 'white',
                    padding: '0.75rem 1.5rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: submitting ? 'not-allowed' : 'pointer',
                    fontWeight: '500',
                    fontSize: '0.95rem',
                  }}
                >
                  {submitting ? 'Submitting...' : '✅ Submit Review'}
                </button>
              </div>
            </form>
          </div>
        )}
      </div>
    </div>
  );
}
