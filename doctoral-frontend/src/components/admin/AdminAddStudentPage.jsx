import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminAddStudentPage() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    FullName: '',
    Email: '',
    IndexNumber: '',
    EnrollmentDate: new Date().toISOString().split('T')[0],
    GPA: 8.0,
    EnglishCertificate: '',
    TotalCreditsFromBachelor: 0,
    TotalCreditsFromMaster: 0,
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'GPA' || name === 'TotalCreditsFromBachelor' || name === 'TotalCreditsFromMaster'
        ? parseFloat(value) || 0
        : value,
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setSuccess('');

    try {
      // Validate
      if (!formData.FullName.trim()) {
        setError('Full name is required');
        setLoading(false);
        return;
      }
      if (!formData.Email.trim()) {
        setError('Email is required');
        setLoading(false);
        return;
      }
      if (!formData.IndexNumber.trim()) {
        setError('Index number is required');
        setLoading(false);
        return;
      }
      if (formData.GPA < 8.0 || formData.GPA > 10) {
        setError('GPA must be between 8.0 and 10.0');
        setLoading(false);
        return;
      }
      if (formData.TotalCreditsFromBachelor + formData.TotalCreditsFromMaster < 300) {
        setError('Total credits must be at least 300 ECTS');
        setLoading(false);
        return;
      }

      // Convert date to ISO string
      const enrollmentDate = new Date(formData.EnrollmentDate);
      if (isNaN(enrollmentDate.getTime())) {
        setError('Invalid enrollment date');
        setLoading(false);
        return;
      }

      // Create student object matching the CreateStudentCommand
      const studentData = {
        FullName: formData.FullName,
        Email: formData.Email,
        IndexNumber: formData.IndexNumber,
        EnrollmentDate: enrollmentDate.toISOString(), // Send as ISO string
        GPA: formData.GPA,
        EnglishCertificate: formData.EnglishCertificate,
        TotalCreditsFromBachelor: formData.TotalCreditsFromBachelor,
        TotalCreditsFromMaster: formData.TotalCreditsFromMaster,
      };

      console.log('Sending student data:', studentData);

      const result = await studentService.createStudent(studentData);

      if (result.id || result.Id) {
        const studentId = result.id || result.Id;
        setSuccess(`Student "${formData.FullName}" created successfully! ID: ${studentId}`);
        setFormData({
          FullName: '',
          Email: '',
          IndexNumber: '',
          EnrollmentDate: new Date().toISOString().split('T')[0],
          GPA: 8.0,
          EnglishCertificate: '',
          TotalCreditsFromBachelor: 0,
          TotalCreditsFromMaster: 0,
        });
      }
    } catch (err) {
      console.error('Full error:', err);
      console.error('Error response data:', err.response?.data);
      
      let errorMsg = 'Failed to create student';
      
      if (err.response?.data) {
        // Handle ASP.NET Core ProblemDetails format
        if (err.response.data.title) {
          errorMsg = err.response.data.title;
          
          // Include validation errors if they exist
          if (err.response.data.errors) {
            const validationErrors = err.response.data.errors;
            const errorList = Object.entries(validationErrors)
              .map(([field, errors]) => `${field}: ${Array.isArray(errors) ? errors.join(', ') : errors}`)
              .join('; ');
            errorMsg += ` - ${errorList}`;
          }
        }
        // Handle custom error object with message property
        else if (err.response.data.message) {
          errorMsg = err.response.data.message;
        }
        // Handle simple string error
        else if (typeof err.response.data === 'string') {
          errorMsg = err.response.data;
        }
        // If it's an object but we don't know the structure, stringify it
        else {
          errorMsg = JSON.stringify(err.response.data);
        }
      } else if (err.message) {
        errorMsg = err.message;
      }
      
      setError(errorMsg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '700px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Add New Student
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Register a new doctoral student
          </p>
        </div>

        {/* Messages */}
        {error && (
          <div style={{
            backgroundColor: '#fef2f2',
            border: '1px solid #fecaca',
            color: '#b91c1c',
            padding: '1rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
          }}>
            <strong>Error:</strong> {error}
          </div>
        )}

        {success && (
          <div style={{
            backgroundColor: '#f0fdf4',
            border: '1px solid #86efac',
            color: '#166534',
            padding: '1.5rem',
            borderRadius: '0.5rem',
            marginBottom: '1rem',
            textAlign: 'center',
          }}>
            <p style={{ margin: '0 0 1rem 0', fontSize: '1rem' }}>
              ✅ {success}
            </p>
            <button
              onClick={() => navigate('/dashboard')}
              style={{
                backgroundColor: '#0d9488',
                color: 'white',
                padding: '0.75rem 1.5rem',
                borderRadius: '0.5rem',
                border: 'none',
                cursor: 'pointer',
                fontWeight: '600',
                fontSize: '0.875rem',
              }}
              onMouseEnter={(e) => {
                e.currentTarget.style.backgroundColor = '#0f766e';
              }}
              onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = '#0d9488';
              }}
            >
              Back to Admin Dashboard
            </button>
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          padding: '2rem',
          border: '1px solid #e5e7eb',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
        }}>
          {/* Full Name */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Full Name *
            </label>
            <input
              type="text"
              name="FullName"
              value={formData.FullName}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="John Doe"
              required
            />
          </div>

          {/* Email */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Email *
            </label>
            <input
              type="email"
              name="Email"
              value={formData.Email}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="john@example.com"
              required
            />
          </div>

          {/* Index Number */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Index Number *
            </label>
            <input
              type="text"
              name="IndexNumber"
              value={formData.IndexNumber}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="PHD2024001"
              required
            />
          </div>

          {/* Enrollment Date */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Enrollment Date *
            </label>
            <input
              type="date"
              name="EnrollmentDate"
              value={formData.EnrollmentDate}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              required
            />
          </div>

          {/* GPA */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              GPA (8.0 - 10.0) *
            </label>
            <input
              type="number"
              name="GPA"
              value={formData.GPA}
              onChange={handleChange}
              min="8"
              max="10"
              step="0.1"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              required
            />
          </div>

          {/* Bachelor Credits */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Bachelor ECTS Credits *
            </label>
            <input
              type="number"
              name="TotalCreditsFromBachelor"
              value={formData.TotalCreditsFromBachelor}
              onChange={handleChange}
              min="0"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="180"
              required
            />
          </div>

          {/* Master Credits */}
          <div style={{ marginBottom: '1.5rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              Master ECTS Credits *
            </label>
            <input
              type="number"
              name="TotalCreditsFromMaster"
              value={formData.TotalCreditsFromMaster}
              onChange={handleChange}
              min="0"
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="120"
              required
            />
          </div>

          {/* English Certificate */}
          <div style={{ marginBottom: '2rem' }}>
            <label style={{
              display: 'block',
              fontSize: '0.875rem',
              fontWeight: '500',
              color: '#374151',
              marginBottom: '0.5rem',
            }}>
              English Certificate (Optional)
            </label>
            <input
              type="text"
              name="EnglishCertificate"
              value={formData.EnglishCertificate}
              onChange={handleChange}
              style={{
                width: '100%',
                padding: '0.75rem',
                border: '1px solid #d1d5db',
                borderRadius: '0.5rem',
                fontSize: '1rem',
                boxSizing: 'border-box',
              }}
              placeholder="TOEFL 100, IELTS 7.5, etc."
            />
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={loading}
            style={{
              width: '100%',
              background: 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
              color: 'white',
              fontWeight: '600',
              padding: '0.75rem',
              borderRadius: '0.5rem',
              border: 'none',
              cursor: loading ? 'not-allowed' : 'pointer',
              opacity: loading ? 0.5 : 1,
              fontSize: '1rem',
            }}
          >
            {loading ? 'Creating Student...' : 'Create Student'}
          </button>
        </form>
      </div>
    </div>
  );
}