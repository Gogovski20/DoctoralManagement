import React from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const getUserRole = () => {
    if (!user?.token) return 'Student';
    try {
      const parts = user.token.split('.');
      if (parts.length !== 3) return 'Student';
      const decoded = JSON.parse(atob(parts[1]));
      return decoded.role || decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 'Student';
    } catch (err) {
      return 'Student';
    }
  };

  const userRole = getUserRole();

  if (location.pathname === '/login' || location.pathname === '/register') {
    return null;
  }

  return (
    <nav style={{
      background: 'linear-gradient(90deg, #0d9488 0%, #0f766e 100%)',
      boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)',
      padding: '0',
    }}>
      <div style={{ maxWidth: '1200px', margin: '0 auto', padding: '0 1rem' }}>
        <div style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          height: '4rem',
        }}>
          {/* Left: Logo and Title */}
          <Link to="/dashboard" style={{
            display: 'flex',
            alignItems: 'center',
            gap: '0.5rem',
            textDecoration: 'none',
          }}>
            <div style={{ fontSize: '1.5rem', fontWeight: 'bold', color: 'white' }}>📚</div>
            <span style={{
              fontSize: '1.125rem',
              fontWeight: 'bold',
              color: 'white',
              display: 'none',
            }} className="hidden sm:inline">
              Doctoral Management
            </span>
          </Link>

          {/* Right: Navigation Links, User Email and Logout */}
          <div style={{
            display: 'flex',
            alignItems: 'center',
            gap: '1rem',
          }}>
            {/* Admin Links */}
            {user && userRole === 'Admin' && (
              <div style={{ 
                display: 'flex', 
                gap: '1rem',
                marginRight: '1rem',
                borderRight: '1px solid rgba(255,255,255,0.3)',
                paddingRight: '1rem',
              }}>
                <Link
                  to="/admin/students/new"
                  style={{
                    color: 'white',
                    textDecoration: 'none',
                    fontSize: '0.875rem',
                    padding: '0.25rem 0.5rem',
                    borderRadius: '0.25rem',
                    transition: 'background-color 0.2s',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
                >
                  Add Student
                </Link>
                <Link
                  to="/admin/programs/new"
                  style={{
                    color: 'white',
                    textDecoration: 'none',
                    fontSize: '0.875rem',
                    padding: '0.25rem 0.5rem',
                    borderRadius: '0.25rem',
                    transition: 'background-color 0.2s',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
                >
                  Add Program
                </Link>
                <Link
                  to="/admin/mentors/new"
                  style={{
                    color: 'white',
                    textDecoration: 'none',
                    fontSize: '0.875rem',
                    padding: '0.25rem 0.5rem',
                    borderRadius: '0.25rem',
                    transition: 'background-color 0.2s',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
                >
                  Add Mentor
                </Link>
              </div>
            )}

            {/* Student-specific links */}
            {user && userRole === 'Student' && (
              <div style={{ 
                display: 'flex', 
                gap: '1rem',
                marginRight: '1rem',
                borderRight: '1px solid rgba(255,255,255,0.3)',
                paddingRight: '1rem',
              }}>
                <Link
                  to="/applications/new"
                  style={{
                    color: 'white',
                    textDecoration: 'none',
                    fontSize: '0.875rem',
                    padding: '0.25rem 0.5rem',
                    borderRadius: '0.25rem',
                    transition: 'background-color 0.2s',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = 'rgba(255,255,255,0.1)'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = 'transparent'}
                >
                  New Application
                </Link>
              </div>
            )}

            {/* User Email and Logout */}
            {user && (
              <>
                <span style={{
                  color: 'white',
                  fontSize: '0.875rem',
                  maxWidth: '200px',
                  overflow: 'hidden',
                  textOverflow: 'ellipsis',
                  whiteSpace: 'nowrap',
                }}>
                  {user.email} ({userRole})
                </span>
                <button
                  onClick={handleLogout}
                  style={{
                    backgroundColor: '#ef4444',
                    color: 'white',
                    padding: '0.5rem 1rem',
                    borderRadius: '0.5rem',
                    border: 'none',
                    cursor: 'pointer',
                    fontSize: '0.875rem',
                    fontWeight: '500',
                    transition: 'background-color 0.2s',
                  }}
                  onMouseEnter={(e) => e.target.style.backgroundColor = '#dc2626'}
                  onMouseLeave={(e) => e.target.style.backgroundColor = '#ef4444'}
                >
                  Logout
                </button>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}