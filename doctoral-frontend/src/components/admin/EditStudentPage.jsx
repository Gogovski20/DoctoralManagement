import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function EditStudentPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [student, setStudent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    gpa: 0,
    englishCertificate: '',
    studentStatus: 'Active',
  });

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const studentData = await studentService.getStudentById(id);
      
      setStudent(studentData);
      setFormData({
        fullName: studentData.fullName || '',
        email: studentData.email || '',
        gpa: studentData.gpa || 0,
        englishCertificate: studentData.englishCertificate || '',
        studentStatus: studentData.studentStatus || 'Active',
      });
    } catch (err) {
      console.error('Failed to fetch data:', err);
      setError('Failed to load student data');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e) => {
    const { name, value, type } = e.target;
    setFormData({
      ...formData,
      [name]: type === 'number' ? parseFloat(value) : value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!formData.fullName.trim()) {
      setError('Full name is required');
      return;
    }

    if (!formData.email.trim()) {
      setError('Email is required');
      return;
    }

    if (formData.gpa < 8 || formData.gpa > 10) {
      setError('GPA must be between 8 and 10');
      return;
    }

    try {
      setSaving(true);
      setError('');
      await studentService.updateStudent(id, formData);
      alert('Student updated successfully');
      navigate('/admin/students');
    } catch (err) {
      console.error('Failed to update student:', err);
      setError(err.response?.data?.message || 'Failed to update student');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Loading student data...</p>
      </div>
    );
  }

  if (!student) {
    return (
      <div style={{ padding: '2rem', textAlign: 'center' }}>
        <p>Student not found</p>
        <Link to="/admin/students" style={{ color: '#0d9488' }}>
          Back to Students
        </Link>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '800px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/admin/students" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Students
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            Edit Student
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Update student information
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

        {/* Form Card */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
        }}>
          <form onSubmit={handleSubmit}>
            {/* Full Name */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Full Name *
              </label>
              <input
                type="text"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Email */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Email *
              </label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* GPA */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                GPA (8-10) *
              </label>
              <input
                type="number"
                name="gpa"
                min="8"
                max="10"
                step="0.01"
                value={formData.gpa}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* English Certificate */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                English Certificate
              </label>
              <input
                type="text"
                name="englishCertificate"
                value={formData.englishCertificate}
                onChange={handleChange}
                placeholder="e.g., TOEFL 100, IELTS 7.0"
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  boxSizing: 'border-box',
                }}
              />
            </div>

            {/* Student Status */}
            <div style={{ marginBottom: '1.5rem' }}>
              <label style={{ display: 'block', fontSize: '0.875rem', fontWeight: '500', color: '#374151', marginBottom: '0.5rem' }}>
                Status *
              </label>
              <select
                name="studentStatus"
                value={formData.studentStatus}
                onChange={handleChange}
                required
                style={{
                  width: '100%',
                  padding: '0.75rem',
                  border: '1px solid #d1d5db',
                  borderRadius: '0.5rem',
                  fontSize: '0.875rem',
                  backgroundColor: 'white',
                  boxSizing: 'border-box',
                }}
              >
                <option value="Active">Active</option>
                <option value="Inactive">Inactive</option>
                <option value="Suspended">Suspended</option>
                <option value="Graduated">Graduated</option>
              </select>
            </div>

            {/* Submit Button */}
            <div style={{ display: 'flex', gap: '1rem' }}>
              <button
                type="submit"
                disabled={saving}
                style={{
                  backgroundColor: '#0d9488',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: saving ? 'not-allowed' : 'pointer',
                  fontWeight: '500',
                  opacity: saving ? 0.5 : 1,
                }}
              >
                {saving ? 'Saving...' : 'Save Changes'}
              </button>
              <Link
                to="/admin/students"
                style={{
                  backgroundColor: '#e5e7eb',
                  color: '#1f2937',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  textDecoration: 'none',
                  fontWeight: '500',
                  display: 'inline-flex',
                  alignItems: 'center',
                }}
              >
                Cancel
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
