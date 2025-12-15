import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { studentService } from '../../api/studentService';

export default function AdminDoctoralProjectPage() {
  const { id } = useParams();
  const [project, setProject] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadProject();
  }, []);

  const loadProject = async () => {
    try {
      const data = await studentService.getDoctoralProjectById(id);
      setProject(data);
    } catch (err) {
      console.error('Failed to load project', err);
    } finally {
      setLoading(false);
    }
  };

  const downloadDocument = async (doc) => {
    await studentService.downloadThesisDocument(
      project.id,
      doc.id,
      doc.fileName
    );
  };

  if (loading) return <p>Loading...</p>;
  if (!project) return <p>Project not found</p>;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f9fafb', padding: '2rem' }}>
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        <Link to="/admin/doctoral-projects" style={{ color: '#0d9488' }}>
          ← Back to projects
        </Link>

        <h1 style={{ fontSize: '2rem', fontWeight: 'bold', marginTop: '1rem' }}>
          {project.title}
        </h1>

        <p style={{ color: '#6b7280' }}>{project.researchArea}</p>

        {/* INFO CARD */}
        <div style={{
          background: 'white',
          padding: '1.5rem',
          borderRadius: '0.75rem',
          border: '1px solid #e5e7eb',
          marginTop: '1.5rem'
        }}>
          <p><strong>Status:</strong> {project.status}</p>
          <p><strong>Student:</strong> {project.studentName}</p>
          <p><strong>Mentor:</strong> {project.mentorName}</p>
        </div>

        {/* DOCUMENTS */}
        <div style={{
          background: 'white',
          padding: '1.5rem',
          borderRadius: '0.75rem',
          border: '1px solid #e5e7eb',
          marginTop: '1.5rem'
        }}>
          <h2>Documents</h2>

          {project.documents.length === 0 && <p>No documents uploaded.</p>}

          {project.documents.map(doc => (
            <div key={doc.id} style={{
              border: '1px solid #e5e7eb',
              borderRadius: '0.5rem',
              padding: '1rem',
              marginTop: '0.75rem'
            }}>
              <strong>{doc.fileName}</strong>
              <p>Type: {doc.type}</p>

              <div style={{ marginTop: '0.5rem', display: 'flex', gap: '0.5rem' }}>
                <button
                  onClick={() => downloadDocument(doc)}
                  style={{
                    background: '#2563eb',
                    color: 'white',
                    border: 'none',
                    borderRadius: '0.4rem',
                    padding: '0.4rem 0.8rem'
                  }}
                >
                  Download
                </button>

                {project.status === 'Completed' &&
                  doc.type === 'DefenseThesisDocument' && (
                    <Link
                      to={`/admin/thesis-review/${project.id}/${doc.id}`}
                      style={{
                        background: '#7c3aed',
                        color: 'white',
                        borderRadius: '0.4rem',
                        padding: '0.4rem 0.8rem',
                        textDecoration: 'none'
                      }}
                    >
                      Review Thesis
                    </Link>
                  )}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
