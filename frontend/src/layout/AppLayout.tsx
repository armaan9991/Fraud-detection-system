import { NavLink, Outlet, useNavigate, useLocation } from 'react-router-dom';
import { useAuthStore } from '../store/authstore';
import { logout } from '../api/authApi';
import styles from './AppLayout.module.css';

const userNavLinks = [
  { to: '/dashboard', icon: '📊', label: 'Dashboard' },
  { to: '/transactions', icon: '💳', label: 'Transactions' },
  { to: '/alerts', icon: '🚨', label: 'Fraud Alerts' },
];

const adminNavLinks = [
  { to: '/dashboard', icon: '📊', label: 'Dashboard' },
  { to: '/transactions', icon: '💳', label: 'Transactions' },
  { to: '/alerts', icon: '🚨', label: 'Fraud Alerts' },
  { to: '/admin/users', icon: '👥', label: 'Users' },
  { to: '/admin/transactions', icon: '⚙️', label: 'Admin' },
];

const pageTitles: Record<string, string> = {
  '/dashboard': 'Dashboard',
  '/transactions': 'Transactions',
  '/alerts': 'Fraud Alerts',
  '/admin/users': 'User Management',
  '/admin/transactions': 'Admin Panel',
};

export default function AppLayout() {
  const { email, role, logout: storeLogout } = useAuthStore();
  const navigate = useNavigate();
  const location = useLocation();

  const navLinks = role === 'Admin' ? adminNavLinks : userNavLinks;
  const pageTitle = pageTitles[location.pathname] ?? 'Dashboard';
  const initials = email ? email.slice(0, 2).toUpperCase() : 'U';

  const handleLogout = async () => {
    const refreshToken = useAuthStore.getState().refreshToken;
    if (refreshToken) {
      try { await logout(refreshToken); } catch { /* ignore */ }
    }
    storeLogout();
    navigate('/login');
  };

  return (
    <div className={styles.appLayoutWrapper}>

      {/* Sidebar */}
      <aside className={styles.appLayoutSidebar}>
        <div className={styles.sidebarLogoArea}>
          <div className={styles.sidebarLogoIcon}>🛡️</div>
          <span className={styles.sidebarLogoText}>FraudDetect</span>
        </div>

        <nav className={styles.sidebarNav}>
          <span className={styles.sidebarNavLabel}>Menu</span>
          {navLinks.map((link) => (
            <NavLink
              key={link.to}
              to={link.to}
              className={({ isActive }) =>
                `${styles.sidebarNavLink} ${isActive ? styles.sidebarNavLinkActive : ''}`
              }
            >
              <span className={styles.sidebarNavIcon}>{link.icon}</span>
              {link.label}
            </NavLink>
          ))}
        </nav>

        <div className={styles.sidebarUserArea}>
          <div className={styles.sidebarUserInfo}>
            <div className={styles.sidebarUserAvatar}>{initials}</div>
            <span className={styles.sidebarUserEmail}>{email}</span>
          </div>
          <button className={styles.sidebarLogoutBtn} onClick={handleLogout}>
            <span className={styles.sidebarNavIcon}>🚪</span>
            Sign out
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className={styles.appLayoutMain}>
        <header className={styles.appLayoutTopbar}>
          <span className={styles.topbarTitle}>{pageTitle}</span>
          <span className={styles.topbarRole}>{role}</span>
        </header>
        <main className={styles.appLayoutContent}>
          <Outlet />
        </main>
      </div>

    </div>
  );
}
