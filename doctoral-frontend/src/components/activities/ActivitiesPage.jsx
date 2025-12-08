import React, { useState } from 'react';
import { Link } from 'react-router-dom';

export default function ActivitiesPage() {
  const [selectedActivity, setSelectedActivity] = useState(null);

  const activities = [
    {
      key: 'publications',
      title: 'Publications',
      description: 'Manage your journal articles, conference papers, and other scientific outputs.',
      icon: '📚',
      gradient: 'linear-gradient(135deg, #0d9488 0%, #0f766e 100%)',
    },
    {
      key: 'mobilities',
      title: 'Mobilities',
      description: 'Track your research stays, exchanges, and international visits.',
      icon: '✈️',
      gradient: 'linear-gradient(135deg, #3b82f6 0%, #1e40af 100%)',
    },
    {
      key: 'conferences',
      title: 'Conference Participations',
      description: 'Record conferences, workshops, and seminars you attend or present at.',
      icon: '🎤',
      gradient: 'linear-gradient(135deg, #f97316 0%, #ea580c 100%)',
    },
  ];

  const handleBack = () => {
    setSelectedActivity(null);
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
              Activities
            </h1>
            <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
              Manage your academic activities
            </p>
          </div>
          <Link
            to="/"
            style={{
              textDecoration: 'none',
              backgroundColor: '#e5e7eb',
              color: '#1f2937',
              padding: '0.5rem 1rem',
              borderRadius: '0.5rem',
              fontSize: '0.875rem',
              fontWeight: 500,
            }}
          >
            ← Back to Dashboard
          </Link>
        </div>

        {/* If nothing selected: show cards */}
        {!selectedActivity && (
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
              gap: '1rem',
            }}
          >
            {activities.map((activity) => {
              // Only conferences goes to separate page, others use local state
              if (activity.key === 'conferences') {
                return (
                  <Link 
                    key={activity.key} 
                    to="/activities/conferences" 
                    style={{ textDecoration: 'none', height: '100%' }}
                  >
                    <div
                      style={{
                        background: activity.gradient,
                        color: 'white',
                        padding: '1.5rem',
                        borderRadius: '0.75rem',
                        boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
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
                      <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>{activity.icon}</div>
                      <h3 style={{ fontWeight: 600, margin: 0, fontSize: '1.1rem', marginBottom: '0.5rem' }}>
                        {activity.title}
                      </h3>
                      <p style={{ fontSize: '0.9rem', opacity: 0.9, margin: 0 }}>{activity.description}</p>
                    </div>
                  </Link>
                );
              }

              // Other activities use local state (unchanged)
              return (
                <button
                  key={activity.key}
                  onClick={() => setSelectedActivity(activity)}
                  style={{
                    border: 'none',
                    padding: 0,
                    background: 'transparent',
                    textAlign: 'left',
                    cursor: 'pointer',
                    height: '100%',
                  }}
                >
                  <div
                    style={{
                      background: activity.gradient,
                      color: 'white',
                      padding: '1.5rem',
                      borderRadius: '0.75rem',
                      boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
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
                    <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>{activity.icon}</div>
                    <h3 style={{ fontWeight: 600, margin: 0, fontSize: '1.1rem', marginBottom: '0.5rem' }}>
                      {activity.title}
                    </h3>
                    <p style={{ fontSize: '0.9rem', opacity: 0.9, margin: 0 }}>{activity.description}</p>
                  </div>
                </button>
              );
            })}
          </div>
        )}

        {/* If one is selected: show detail placeholder */}
        {selectedActivity && (
          <div
            style={{
              backgroundColor: 'white',
              borderRadius: '0.75rem',
              boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
              border: '1px solid #e5e7eb',
              padding: '1.5rem',
            }}
          >
            <div
              style={{
                marginBottom: '1.5rem',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'space-between',
              }}
            >
              <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                <span style={{ fontSize: '2rem' }}>{selectedActivity.icon}</span>
                <div>
                  <h2
                    style={{
                      fontSize: '1.5rem',
                      fontWeight: 'bold',
                      color: '#1f2937',
                      margin: 0,
                    }}
                  >
                    {selectedActivity.title}
                  </h2>
                  <p style={{ color: '#6b7280', margin: 0 }}>
                    {selectedActivity.description}
                  </p>
                </div>
              </div>
              <button
                onClick={handleBack}
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
                ← Back to activities
              </button>
            </div>

            {/* Placeholder content for future features */}
            <div
              style={{
                padding: '2rem',
                textAlign: 'center',
                borderRadius: '0.75rem',
                border: '1px dashed #d1d5db',
                backgroundColor: '#f9fafb',
              }}
            >
              <div style={{ fontSize: '2.5rem', marginBottom: '1rem' }}>🚧</div>
              <h3
                style={{
                  fontSize: '1.25rem',
                  fontWeight: 600,
                  color: '#1f2937',
                  marginBottom: '0.5rem',
                }}
              >
                {selectedActivity.title} will be here soon
              </h3>
              <p style={{ color: '#6b7280', fontSize: '0.95rem', marginBottom: '1rem' }}>
                This section will allow you to add, edit, and track your{' '}
                {selectedActivity.title.toLowerCase()}.
              </p>
              <button
                disabled
                style={{
                  background: 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
                  color: 'white',
                  padding: '0.75rem 1.5rem',
                  borderRadius: '0.5rem',
                  border: 'none',
                  cursor: 'not-allowed',
                  fontWeight: '500',
                  fontSize: '0.95rem',
                  opacity: 0.6,
                }}
              >
                + Add {selectedActivity.title}
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
