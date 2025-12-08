import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function ECTSTrackingPage() {
  const [ectsStatus, setEctsStatus] = useState(null);
  const [ectsDetailed, setEctsDetailed] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('overview');

  useEffect(() => {
    fetchEctsData();
  }, []);

  const fetchEctsData = async () => {
    try {
      setLoading(true);
      setError('');
      
      const [statusData, detailedData] = await Promise.all([
        studentService.getEctsStatus(),
        studentService.getEctsDetailed(),
      ]);
      
      setEctsStatus(statusData);
      setEctsDetailed(detailedData);
    } catch (err) {
      console.error('Failed to fetch ECTS data:', err);
      
      // Check if this is a "no data" error (404 or 500 because record doesn't exist)
      // vs a real server error
      const status = err.response?.status;
      const isNoDataError = status === 404 || status === 500;
      
      if (isNoDataError) {
        // Don't show error - just leave data as null
        // This will trigger the "no data" message below
        setEctsStatus(null);
        setEctsDetailed(null);
      } else {
        // Real error - show to user
        setError(
          err.response?.data?.message || 
          'Failed to load ECTS tracking data'
        );
      }
    } finally {
      setLoading(false);
    }
  };

  const getProgressBarColor = (progressPercent) => {
    if (progressPercent >= 100) return '#10b981'; // Green
    if (progressPercent >= 70) return '#3b82f6'; // Blue
    if (progressPercent >= 40) return '#f59e0b'; // Amber
    return '#ef4444'; // Red
  };

  const formatProgressPercent = (percent) => {
    return Math.min(100, Math.round(percent * 10) / 10);
  };

  if (loading) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto', textAlign: 'center', paddingTop: '3rem' }}>
          <p>Loading ECTS tracking data...</p>
        </div>
      </div>
    );
  }

  if (!ectsStatus || !ectsDetailed) {
    return (
      <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
        <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
          <Link to="/dashboard" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Dashboard
          </Link>
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '3rem',
            textAlign: 'center',
            color: '#6b7280',
          }}>
            <p style={{ fontSize: '3rem', marginBottom: '1rem' }}>📚</p>
            <h2 style={{ color: '#1f2937', marginBottom: '0.5rem' }}>No ECTS Tracking Yet</h2>
            <p style={{ marginBottom: '1.5rem' }}>
              ECTS tracking becomes available once your application is accepted and you're formally enrolled as a doctoral student.
            </p>
            <Link to="/dashboard" style={{
              backgroundColor: '#0d9488',
              color: 'white',
              padding: '0.75rem 1.5rem',
              borderRadius: '0.5rem',
              textDecoration: 'none',
              fontWeight: '500',
              display: 'inline-block',
            }}>
              Back to Dashboard
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <Link to="/dashboard" style={{ color: '#0d9488', marginBottom: '1rem', display: 'inline-block' }}>
            ← Back to Dashboard
          </Link>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: '0.5rem 0 0 0' }}>
            ECTS Tracking
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Monitor your doctoral study progress
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

        {/* Overall Progress */}
        <div style={{
          backgroundColor: 'white',
          borderRadius: '0.75rem',
          boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          padding: '1.5rem',
          marginBottom: '1.5rem',
        }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '1.5rem' }}>
            {/* Total ECTS Card */}
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.5rem 0' }}>Total ECTS Acquired</p>
              <div style={{ display: 'flex', alignItems: 'baseline', gap: '0.5rem', marginBottom: '1rem' }}>
                <div style={{ fontSize: '2.5rem', fontWeight: 'bold', color: '#1f2937' }}>
                  {ectsStatus.totalEcts}
                </div>
                <div style={{ fontSize: '1rem', color: '#6b7280' }}>/180</div>
              </div>
              <div style={{
                width: '100%',
                height: '0.5rem',
                backgroundColor: '#e5e7eb',
                borderRadius: '9999px',
                overflow: 'hidden',
              }}>
                <div style={{
                  height: '100%',
                  width: `${Math.min(100, (ectsStatus.totalEcts / 180) * 100)}%`,
                  backgroundColor: getProgressBarColor((ectsStatus.totalEcts / 180) * 100),
                  transition: 'width 0.3s ease',
                }}
                />
              </div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0.5rem 0 0 0' }}>
                {formatProgressPercent((ectsStatus.totalEcts / 180) * 100)}% Complete
              </p>
            </div>

            {/* Current Semester */}
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.5rem 0' }}>Current Semester</p>
              <div style={{
                fontSize: '2.5rem',
                fontWeight: 'bold',
                color: '#0d9488',
                marginBottom: '1rem',
              }}>
                {ectsStatus.currentSemester}
              </div>
              <span style={{
                display: 'inline-block',
                padding: '0.25rem 0.75rem',
                borderRadius: '9999px',
                backgroundColor: ectsStatus.isCompleted ? '#dcfce7' : '#fef3c7',
                color: ectsStatus.isCompleted ? '#166534' : '#92400e',
                fontWeight: '500',
                fontSize: '0.875rem',
              }}>
                {ectsStatus.isCompleted ? '✓ Completed' : '🔄 In Progress'}
              </span>
            </div>

            {/* Status Card */}
            <div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.5rem 0' }}>Progress Status</p>
              <div style={{
                fontSize: '1.5rem',
                fontWeight: 'bold',
                marginBottom: '1rem',
              }}>
                <span style={{
                  display: 'inline-block',
                  padding: '0.5rem 1rem',
                  borderRadius: '0.5rem',
                  backgroundColor: getProgressBarColor(ectsStatus.progressPercent) + '20',
                  color: getProgressBarColor(ectsStatus.progressPercent),
                }}>
                  {formatProgressPercent(ectsStatus.progressPercent)}%
                </span>
              </div>
              <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
                {ectsStatus.totalEcts >= 180 ? '🎉 All ECTS acquired!' : `${180 - ectsStatus.totalEcts} ECTS remaining`}
              </p>
            </div>
          </div>
        </div>

        {/* Tab Navigation */}
        <div style={{
          display: 'flex',
          gap: '1rem',
          marginBottom: '1.5rem',
          borderBottom: '1px solid #e5e7eb',
        }}>
          <button
            onClick={() => setActiveTab('overview')}
            style={{
              padding: '1rem 1.5rem',
              border: 'none',
              backgroundColor: 'transparent',
              cursor: 'pointer',
              fontWeight: activeTab === 'overview' ? '600' : '500',
              color: activeTab === 'overview' ? '#0d9488' : '#6b7280',
              borderBottom: activeTab === 'overview' ? '2px solid #0d9488' : '2px solid transparent',
              transition: 'all 0.2s',
            }}
          >
            Overview
          </button>
          <button
            onClick={() => setActiveTab('detailed')}
            style={{
              padding: '1rem 1.5rem',
              border: 'none',
              backgroundColor: 'transparent',
              cursor: 'pointer',
              fontWeight: activeTab === 'detailed' ? '600' : '500',
              color: activeTab === 'detailed' ? '#0d9488' : '#6b7280',
              borderBottom: activeTab === 'detailed' ? '2px solid #0d9488' : '2px solid transparent',
              transition: 'all 0.2s',
            }}
          >
            Detailed Breakdown
          </button>
        </div>

        {/* Overview Tab */}
        {activeTab === 'overview' && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1.5rem' }}>
              ECTS Summary
            </h2>

            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', gap: '1rem' }}>
              <EctsCard
                label="Organized Academic Training"
                current={ectsStatus.organizedAcademicTraining}
                required={42}
                color="#3b82f6"
              />
              <EctsCard
                label="Independent Research Project"
                current={ectsStatus.independentResearchProject}
                required={41}
                color="#8b5cf6"
              />
              <EctsCard
                label="International Mobility"
                current={ectsStatus.internationalMobility}
                required={6}
                color="#ec4899"
              />
              <EctsCard
                label="Teaching Activities"
                current={ectsStatus.teachingActivities}
                required={18}
                color="#f59e0b"
              />
              <EctsCard
                label="Publications"
                current={ectsStatus.publications}
                required={27}
                color="#10b981"
              />
              <EctsCard
                label="Thesis Defence"
                current={ectsStatus.thesisDefence}
                required={46}
                color="#06b6d4"
              />
            </div>
          </div>
        )}

        {/* Detailed Breakdown Tab */}
        {activeTab === 'detailed' && ectsDetailed && (
          <div style={{
            backgroundColor: 'white',
            borderRadius: '0.75rem',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
            border: '1px solid #e5e7eb',
            padding: '1.5rem',
          }}>
            <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1.5rem' }}>
              Detailed Category Breakdown
            </h2>

            <div style={{ display: 'flex', flexDirection: 'column', gap: '2rem' }}>
              {ectsDetailed.categories.map((category, index) => (
                <DetailedEctsCard key={index} category={category} />
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function EctsCard({ label, current, required, color }) {
  const progressPercent = (current / required) * 100;
  const isComplete = current >= required;

  return (
    <div style={{
      border: `1px solid ${color}40`,
      borderRadius: '0.75rem',
      padding: '1.5rem',
      backgroundColor: `${color}05`,
    }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '1rem' }}>
        <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0, fontWeight: '500' }}>
          {label}
        </p>
        <span style={{
          display: 'inline-block',
          padding: '0.25rem 0.75rem',
          borderRadius: '9999px',
          backgroundColor: isComplete ? '#dcfce7' : '#fee2e2',
          color: isComplete ? '#166534' : '#991b1b',
          fontWeight: '600',
          fontSize: '0.75rem',
        }}>
          {isComplete ? '✓' : '○'}
        </span>
      </div>

      <div style={{ display: 'flex', alignItems: 'baseline', gap: '0.5rem', marginBottom: '0.75rem' }}>
        <div style={{ fontSize: '1.5rem', fontWeight: 'bold', color }}>
          {current}
        </div>
        <div style={{ color: '#6b7280', fontSize: '0.875rem' }}>/{required}</div>
      </div>

      <div style={{
        width: '100%',
        height: '0.5rem',
        backgroundColor: '#e5e7eb',
        borderRadius: '9999px',
        overflow: 'hidden',
      }}>
        <div style={{
          height: '100%',
          width: `${Math.min(100, progressPercent)}%`,
          backgroundColor: color,
          transition: 'width 0.3s ease',
        }}
        />
      </div>

      <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0.5rem 0 0 0' }}>
        {formatProgressPercent(progressPercent)}% complete
      </p>
    </div>
  );
}

function DetailedEctsCard({ category }) {
  const progressPercent = category.progressPercent;

  return (
    <div style={{
      border: '1px solid #e5e7eb',
      borderRadius: '0.75rem',
      padding: '1.5rem',
    }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '1rem' }}>
        <div>
          <h3 style={{ fontWeight: '600', color: '#1f2937', margin: 0, marginBottom: '0.25rem' }}>
            {category.categoryName}
          </h3>
          <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: 0 }}>
            Required ECTS: {category.required}
          </p>
        </div>
        <span style={{
          display: 'inline-block',
          padding: '0.25rem 0.75rem',
          borderRadius: '9999px',
          backgroundColor: category.isComplete ? '#dcfce7' : '#fef3c7',
          color: category.isComplete ? '#166534' : '#92400e',
          fontWeight: '600',
          fontSize: '0.75rem',
        }}>
          {category.isComplete ? '✓ Complete' : '⏳ In Progress'}
        </span>
      </div>

      <div style={{
        display: 'grid',
        gridTemplateColumns: 'repeat(3, 1fr)',
        gap: '1rem',
        marginBottom: '1.5rem',
      }}>
        <div>
          <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0 0 0.5rem 0' }}>Awarded</p>
          <p style={{ fontSize: '1.75rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            {category.awarded}
          </p>
        </div>
        <div>
          <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0 0 0.5rem 0' }}>Required</p>
          <p style={{ fontSize: '1.75rem', fontWeight: 'bold', color: '#6b7280', margin: 0 }}>
            {category.required}
          </p>
        </div>
        <div>
          <p style={{ color: '#6b7280', fontSize: '0.75rem', margin: '0 0 0.5rem 0' }}>Remaining</p>
          <p style={{ fontSize: '1.75rem', fontWeight: 'bold', color: category.remaining > 0 ? '#ef4444' : '#10b981', margin: 0 }}>
            {category.remaining}
          </p>
        </div>
      </div>

      <div style={{
        width: '100%',
        height: '0.75rem',
        backgroundColor: '#e5e7eb',
        borderRadius: '9999px',
        overflow: 'hidden',
      }}>
        <div style={{
          height: '100%',
          width: `${Math.min(100, progressPercent)}%`,
          backgroundColor: category.isComplete ? '#10b981' : '#3b82f6',
          transition: 'width 0.3s ease',
        }}
        />
      </div>

      <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0.75rem 0 0 0' }}>
        {formatProgressPercent(progressPercent)}% progress
      </p>
    </div>
  );
}

function formatProgressPercent(percent) {
  return Math.min(100, Math.round(percent * 10) / 10);
}
