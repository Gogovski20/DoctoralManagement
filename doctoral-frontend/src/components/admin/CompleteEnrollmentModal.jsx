import React, { useState } from 'react';
import { studentService } from '../../api/studentService';

export default function CompleteEnrollmentModal({ enrollment, onClose, onSuccess }) {
  const [grade, setGrade] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const gradeNum = parseFloat(grade);
    if (!grade || isNaN(gradeNum)) {
      setError('Please enter a valid grade');
      return;
    }
    if (gradeNum < 5.0 || gradeNum > 10.0) {
      setError('Grade must be between 5.0 and 10.0');
      return;
    }

    try {
      setLoading(true);
      setError('');

      const payload = {
        grade: gradeNum,
      };

      await studentService.completeCourseEnrollment(
        enrollment.studentId,
        enrollment.id,
        payload
      );

      setSuccess(true);

      setTimeout(() => {
        onSuccess();
        onClose();
      }, 1500);
    } catch (err) {
      setError(
        `Failed to complete enrollment: ${
          err.response?.data?.message || err.message
        }`
      );
    } finally {
      setLoading(false);
    }
  };

  // SUCCESS UI
  if (success) {
    return (
      <div
        style={{
          position: 'fixed',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          backgroundColor: 'rgba(0, 0, 0, 0.5)',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          zIndex: 1000,
          padding: '1rem',
        }}
      >
        <div
          style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 10px 25px rgba(0, 0, 0, 0.2)',
            padding: '2rem',
            maxWidth: '400px',
            width: '100%',
            textAlign: 'center',
          }}
        >
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>✅</div>
          <h2
            style={{
              fontSize: '1.5rem',
              fontWeight: 'bold',
              color: '#1f2937',
              marginBottom: '0.5rem',
            }}
          >
            Enrollment Completed!
          </h2>
          <p style={{ color: '#6b7280' }}>
            Course enrollment has been marked as completed with grade {grade}
          </p>
        </div>
      </div>
    );
  }

  // DEFAULT MODAL UI
  return (
    <div
      style={{
        position: 'fixed',
        top: 0,
        left: 0,
        right: 0,
        bottom: 0,
        backgroundColor: 'rgba(0, 0, 0, 0.5)',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        zIndex: 1000,
        padding: '1rem',
      }}
      onClick={onClose}
    >
      <div
        style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 10px 25px rgba(0, 0, 0, 0.2)',
          padding: '2rem',
          maxWidth: '500px',
          width: '100%',
        }}
        onClick={(e) => e.stopPropagation()}
      >
        {/* Header */}
        <div
          style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            marginBottom: '1.5rem',
          }}
        >
          <h2
            style={{
              fontSize: '1.5rem',
              fontWeight: 'bold',
              color: '#1f2937',
              margin: 0,
            }}
          >
            Complete Course Enrollment
          </h2>
          <button
            onClick={onClose}
            style={{
              backgroundColor: 'transparent',
              border: 'none',
              fontSize: '1.5rem',
              cursor: 'pointer',
              color: '#6b7280',
            }}
          >
            ✕
          </button>
        </div>

        {/* Error */}
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

        {/* Info section */}
        <div
          style={{
            backgroundColor: '#f9fafb',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1.5rem',
          }}
        >
          <div style={{ marginBottom: '0.75rem' }}>
            <label
              style={{
                display: 'block',
                fontWeight: '600',
                color: '#6b7280',
                fontSize: '0.875rem',
                marginBottom: '0.25rem',
              }}
            >
              Student
            </label>
            <p
              style={{
                fontSize: '1rem',
                color: '#1f2937',
                margin: 0,
                fontWeight: '500',
              }}
            >
              {enrollment.studentName}
            </p>
            <p
              style={{
                fontSize: '0.875rem',
                color: '#6b7280',
                margin: '0.25rem 0 0 0',
              }}
            >
              {enrollment.studentIndex}
            </p>
          </div>

          <div style={{ marginBottom: '0.75rem' }}>
            <label
              style={{
                display: 'block',
                fontWeight: '600',
                color: '#6b7280',
                fontSize: '0.875rem',
                marginBottom: '0.25rem',
              }}
            >
              Course
            </label>
            <p
              style={{
                fontSize: '1rem',
                color: '#1f2937',
                margin: 0,
                fontWeight: '500',
              }}
            >
              {enrollment.courseTitle}
            </p>
          </div>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: '1.5rem' }}>
            <label
              style={{
                display: 'block',
                fontWeight: '600',
                color: '#374151',
                marginBottom: '0.5rem',
              }}
            >
              Grade (5.0 - 10.0) *
            </label>
            <input
              type="number"
              step="0.1"
              min="5.0"
              max="10.0"
              value={grade}
              onChange={(e) => setGrade(e.target.value)}
              required
              placeholder="Enter grade (e.g., 8.5)"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
              }}
            />
          </div>

          {/* Buttons */}
          <div
            style={{ display: 'flex', gap: '1rem', justifyContent: 'flex-end' }}
          >
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              style={{
                backgroundColor: '#e5e7eb',
                color: '#1f2937',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: loading ? 'not-allowed' : 'pointer',
                fontWeight: '500',
              }}
            >
              Cancel
            </button>

            <button
              type="submit"
              disabled={loading}
              style={{
                backgroundColor: loading ? '#6b7280' : '#10b981',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: loading ? 'not-allowed' : 'pointer',
                fontWeight: '500',
              }}
            >
              {loading ? 'Completing...' : '✅ Complete Enrollment'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
