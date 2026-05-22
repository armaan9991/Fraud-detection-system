import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts';
import { useAuthStore } from '../store/authstore';
import { getTransactions } from '../api/transactionApi';
import { getAdminStats } from '../api/adminApi';
import type { Transaction } from '../types/common.types';
import styles from './DashboadPage.module.css';

const STATUS_COLORS: Record<string, string> = {
  HIGH: '#EC111A',
  MEDIUM: '#F59E0B',
  LOW: '#10B981',
};

function StatusBadge({ status }: { status: string }) {
  const cls =
    status === 'HIGH' ? styles.dashboardBadgeHigh :
    status === 'MEDIUM' ? styles.dashboardBadgeMedium :
    styles.dashboardBadgeLow;
  return <span className={`${styles.dashboardBadge} ${cls}`}>● {status}</span>;
}

function FraudScoreBar({ score }: { score: number }) {
  const fillClass =
    score >= 60 ? styles.dashboardScoreFillHigh :
    score >= 35 ? styles.dashboardScoreFillMedium :
    styles.dashboardScoreFillLow;
  return (
    <div className={styles.dashboardScoreWrapper}>
      <div className={styles.dashboardScoreBar}>
        <div className={`${styles.dashboardScoreFill} ${fillClass}`} style={{ width: `${score}%` }} />
      </div>
      <span className={styles.dashboardScoreText}>{score}</span>
    </div>
  );
}

function LoadingState() {
  return (
    <div className={styles.dashboardLoading}>
      <div className={styles.dashboardSpinner} />
      Loading...
    </div>
  );
}

function UserDashboard() {
  const { data, isLoading } = useQuery({
    queryKey: ['transactions'],
    queryFn: () => getTransactions(1, 10),
  });

  const transactions = data?.items ?? [];
  const highCount = transactions.filter((t: Transaction) => t.status === 'HIGH').length;
  const mediumCount = transactions.filter((t: Transaction) => t.status === 'MEDIUM').length;

  return (
    <>
      <div className={styles.dashboardStatsGrid}>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconBlue}`}>💳</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Transactions</span>
            <span className={styles.dashboardStatValue}>{data?.totalRecords ?? '—'}</span>
            <span className={styles.dashboardStatFooter}>All time</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconRed}`}>🚨</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>High Risk</span>
            <span className={styles.dashboardStatValue}>{highCount}</span>
            <span className={styles.dashboardStatFooter}>Last 10 transactions</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconYellow}`}>⚠️</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Medium Risk</span>
            <span className={styles.dashboardStatValue}>{mediumCount}</span>
            <span className={styles.dashboardStatFooter}>Last 10 transactions</span>
          </div>
        </div>
      </div>

      <div className={styles.dashboardSectionCard}>
        <div className={styles.dashboardSectionHeader}>
          <h3 className={styles.dashboardSectionTitle}>Recent Transactions</h3>
          <span className={styles.dashboardSectionCount}>{transactions.length} shown</span>
        </div>
        {isLoading ? <LoadingState /> : transactions.length === 0 ? (
          <div className={styles.dashboardEmpty}>
            <span className={styles.dashboardEmptyIcon}>💳</span>
            No transactions yet
          </div>
        ) : (
          <table className={styles.dashboardTable}>
            <thead>
              <tr>
                <th>ID</th><th>Amount</th><th>Country</th>
                <th>Fraud Score</th><th>Status</th><th>Date</th>
              </tr>
            </thead>
            <tbody>
              {transactions.map((t: Transaction) => (
                <tr key={t.transactionId}>
                  <td className={styles.dashboardTableIdCell}>#{t.transactionId}</td>
                  <td><strong>{t.currency}</strong> {t.amount.toLocaleString()}</td>
                  <td>{t.country}</td>
                  <td><FraudScoreBar score={t.fraudScore} /></td>
                  <td><StatusBadge status={t.status} /></td>
                  <td>{new Date(t.transactionTime).toLocaleDateString()}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </>
  );
}

function AdminDashboard() {
  const { data, isLoading } = useQuery({
    queryKey: ['adminStats'],
    queryFn: getAdminStats,
  });

  if (isLoading) return <LoadingState />;

  const chartData = Object.entries(data?.transactionByStatus ?? {}).map(([status, count]) => ({
    status, count,
  }));

  return (
    <>
      <div className={styles.dashboardStatsGrid}>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconNavy}`}>👥</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Users</span>
            <span className={styles.dashboardStatValue}>{data?.totalUsers ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconBlue}`}>💳</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Transactions</span>
            <span className={styles.dashboardStatValue}>{data?.totalTransaction ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconRed}`}>🚨</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Fraud Alerts</span>
            <span className={styles.dashboardStatValue}>{data?.totalFraudAlerts ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconGreen}`}>💰</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Amount</span>
            <span className={styles.dashboardStatValue}>
              ${(data?.totalTransactionAmount ?? 0).toLocaleString()}
            </span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconYellow}`}>📊</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Avg Fraud Score</span>
            <span className={styles.dashboardStatValue}>
              {(data?.AverageFraudScore ?? 0).toFixed(1)}
            </span>
            <span className={styles.dashboardStatFooter}>out of 100</span>
          </div>
        </div>
      </div>

      <div className={styles.dashboardSectionCard}>
        <div className={styles.dashboardSectionHeader}>
          <h3 className={styles.dashboardSectionTitle}>Transactions by Risk Level</h3>
        </div>
        <div className={styles.dashboardChartWrapper}>
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={chartData} barSize={48}>
              <XAxis dataKey="status" tick={{ fontSize: 12, fill: '#6B7280' }} axisLine={false} tickLine={false} />
              <YAxis tick={{ fontSize: 12, fill: '#6B7280' }} axisLine={false} tickLine={false} />
              <Tooltip
                contentStyle={{ background: '#fff', border: '1px solid #E5E7EB', borderRadius: '8px', fontSize: '0.8rem' }}
                cursor={{ fill: 'rgba(0,0,0,0.04)' }}
              />
              <Bar dataKey="count" radius={[6, 6, 0, 0]}>
                {chartData.map((entry) => (
                  <Cell key={entry.status} fill={STATUS_COLORS[entry.status] ?? '#6B7280'} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </>
  );
}

export default function DashboardPage() {
  const { email, role } = useAuthStore();
  const firstName = email?.split('@')[0] ?? 'User';

  return (
    <>
      {/* Navy banner — blends with sidebar */}
      <div className={styles.dashboardBanner}>
        <div className={styles.dashboardBannerInner}>
          <div>
            <h1 className={styles.dashboardWelcome}>Welcome back, {firstName} 👋</h1>
            <p className={styles.dashboardSubtext}>
              {role === 'Admin'
                ? 'Platform overview — all systems monitored'
                : 'Here\'s a summary of your account activity'}
            </p>
          </div>
        </div>
      </div>

      {/* Content pulls up over banner */}
      <div className={styles.dashboardBody}>
        {role === 'Admin' ? <AdminDashboard /> : <UserDashboard />}
      </div>
    </>
  );
}
