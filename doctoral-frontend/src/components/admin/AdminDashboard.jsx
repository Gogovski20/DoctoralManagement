import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import apiClient from '../../api/apiClient';
import { studentService } from '../../api/studentService';

export default function AdminDashboard() {
  const [stats, setStats] = useState({
    studentsCount: 0,
    mentorsCount: 0,
    programsCount: 0,
    applicationsCount: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchStats();
  }, []);

  const fetchStats = async () => {
    try {
      const [students, mentors, programs, applications] = await Promise.all([
        apiClient.get('/Students').catch(() => ({ data: [] })),
        apiClient.get('/Mentors').catch(() => ({ data: [] })),
        apiClient.get('/DoctoralPrograms').catch(() => ({ data: [] })),
        apiClient.get('/Applications').catch(() => ({ data: [] })),
        studentService.getAllApplications().catch(() => ({ data: [] })),
        studentService.getAllStudents().catch(() => ({ data: [] })),
      ]);

      setStats({
        studentsCount: students.data?.length || 0,
        mentorsCount: mentors.data?.length || 0,
        programsCount: programs.data?.length || 0,
        applicationsCount: applications.data?.length || 0,
      });
    } catch (err) {
      console.error('Failed to fetch stats:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Header */}
        <div style={{ marginBottom: '2rem' }}>
          <h1 style={{ fontSize: '2rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>
            Admin Dashboard
          </h1>
          <p style={{ color: '#6b7280', marginTop: '0.5rem' }}>
            Manage students, mentors, programs, and applications
          </p>
        </div>

        {/* Stats Grid */}
        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
            gap: '1.5rem',
            marginBottom: '2rem',
          }}
        >
          <StatCard title="Total Students" count={stats.studentsCount} icon="👨‍🎓" />
          <StatCard title="Total Mentors" count={stats.mentorsCount} icon="👨‍🏫" />
          <StatCard title="Programs" count={stats.programsCount} icon="📚" />
          <StatCard title="Applications" count={stats.applicationsCount} icon="📋" />
        </div>

        {/* Quick Actions */}
        <div style={{ marginBottom: '2rem' }}>
          <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#1f2937', marginBottom: '1rem' }}>
            Management
          </h2>
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
              gap: '1rem',
            }}
          >
            <NavCard to="/admin/students" title="Manage Students" icon="👥" />
            <NavCard to="/admin/students/new" title="Add New Student" icon="➕" />
            <NavCard to="/admin/mentors" title="Manage Mentors" icon="👨‍💼" />
            <NavCard to="/admin/mentors/new" title="Add New Mentor" icon="👨‍🏫" />
            <NavCard to="/admin/programs" title="Manage Programs" icon="🏫" />
            <NavCard to="/admin/programs/new" title="Add New Program" icon="📚" />
            <NavCard to="/admin/courses" title="Manage Courses" icon="📖" />
            <NavCard to="/admin/applications" title="Review Applications" icon="📊" />
            <NavCard to="/admin/doctoral-projects" title="Doctoral Projects" icon="🔬" />

          </div>
        </div>
      </div>
    </div>
  );
}

function StatCard({ title, count, icon }) {
  return (
    <div
      style={{
        backgroundColor: 'white',
        borderRadius: '0.5rem',
        padding: '1.5rem',
        boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
        border: '1px solid #e5e7eb',
      }}
    >
      <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>{icon}</div>
      <p style={{ color: '#6b7280', fontSize: '0.875rem', margin: '0 0 0.5rem 0' }}>{title}</p>
      <p style={{ fontSize: '1.875rem', fontWeight: 'bold', color: '#1f2937', margin: 0 }}>{count}</p>
    </div>
  );
}

function NavCard({ to, title, icon }) {
  return (
    <Link
      to={to}
      style={{
        display: 'block',
        backgroundColor: 'white',
        borderRadius: '0.5rem',
        padding: '1.5rem',
        textDecoration: 'none',
        border: '1px solid #e5e7eb',
        textAlign: 'center',
        transition: 'all 0.2s',
        cursor: 'pointer',
      }}
      onMouseEnter={(e) => {
        e.currentTarget.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1)';
        e.currentTarget.style.borderColor = '#0d9488';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.boxShadow = 'none';
        e.currentTarget.style.borderColor = '#e5e7eb';
      }}
    >
      <div style={{ fontSize: '2rem', marginBottom: '0.5rem' }}>{icon}</div>
      <p style={{ color: '#1f2937', fontWeight: '500', margin: 0 }}>{title}</p>
    </Link>
  );
}
