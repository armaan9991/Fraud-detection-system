import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from 'recharts';
import { useAuthStore } from '../store/authstore';
import { getTransactions } from '../api/transactionApi';
import { getAdminStats } from '../api/adminApi';
import { TransactionDrawer, UserDetailModal, StatusBadge, FraudScoreBar } from '../components/TransactionDrawer';
import type { Transaction } from '../types/common.types';
import styles from './DashboadPage.module.css';

const STATUS_COLORS: Record<string, string> = {
  HIGH: '#EC111A',
  MEDIUM: '#F59E0B',
  LOW: '#10B981',
};

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
  const transactioncount = data?.totalRecords;
  const highCount = transactions.filter((t: Transaction) => t.fraudScore >= 60).length;
  const mediumCount = transactions.filter((t: Transaction) => t.fraudScore >= 35 && t.fraudScore < 60).length;

  return (
    <>
      <div className={styles.dashboardStatsGrid}>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconBlue}`}>💳</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Transactions</span>
            <span className={styles.dashboardStatValue}>{transactioncount ?? '—'}</span>
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
              <tr><th>ID</th><th>Amount</th><th>Country</th><th>Fraud Score</th><th>Status</th><th>Date</th></tr>
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
  const [selectedTx, setSelectedTx] = useState<Transaction | null>(null);
  const [selectedUser, setSelectedUser] = useState<number | null>(null);

  const { data: stats, isLoading: statsLoading } = useQuery({
    queryKey: ['adminStats'],
    queryFn: getAdminStats,
  });

  const { data: txData, isLoading: txLoading } = useQuery({
    queryKey: ['transactions', 1],
    queryFn: () => getTransactions(1, 10),
  });

  if (statsLoading) return <LoadingState />;

  const chartData = Object.entries(stats?.transactionByStatus ?? {}).map(([status, count]) => ({ status, count }));
  const transactions = txData?.items ?? [];

  return (
    <>
      <div className={styles.dashboardStatsGrid}>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconNavy}`}>👥</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Users</span>
            <span className={styles.dashboardStatValue}>{stats?.totalUsers ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconBlue}`}>💳</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Transactions</span>
            <span className={styles.dashboardStatValue}>{stats?.totalTransaction ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconRed}`}>🚨</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Fraud Alerts</span>
            <span className={styles.dashboardStatValue}>{stats?.totalFraudAlerts ?? 0}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconGreen}`}>💰</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Total Amount</span>
            <span className={styles.dashboardStatValue}>${(stats?.totalTransactionAmount ?? 0).toLocaleString()}</span>
          </div>
        </div>
        <div className={styles.dashboardStatCard}>
          <div className={`${styles.dashboardStatIconArea} ${styles.dashboardStatIconYellow}`}>📊</div>
          <div className={styles.dashboardStatBody}>
            <span className={styles.dashboardStatLabel}>Avg Fraud Score</span>
            <span className={styles.dashboardStatValue}>{(stats?.AverageFraudScore ?? 0).toFixed(1)}</span>
            <span className={styles.dashboardStatFooter}>out of 100</span>
          </div>
        </div>
      </div>

      {/* <div className={styles.dashboardSectionCard}>
        <div className={styles.dashboardSectionHeader}>
          <h3 className={styles.dashboardSectionTitle}>Transactions by Status</h3>
        </div>
        <div className={styles.dashboardChartWrapper}>
          <ResponsiveContainer width="100%" height={220}>
            <BarChart data={chartData} barSize={48}>
              <XAxis dataKey="status" tick={{ fontSize: 12, fill: '#6B7280' }} axisLine={false} tickLine={false} />
              <YAxis tick={{ fontSize: 12, fill: '#6B7280' }} axisLine={false} tickLine={false} />
              <Tooltip contentStyle={{ background: '#fff', border: '1px solid #E5E7EB', borderRadius: '8px', fontSize: '0.8rem' }} cursor={{ fill: 'rgba(0,0,0,0.04)' }} />
              <Bar dataKey="count" radius={[6, 6, 0, 0]}>
                {chartData.map((entry) => (
                  <Cell key={entry.status} fill={STATUS_COLORS[entry.status] ?? '#6B7280'} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div> */}

      <div className={styles.dashboardSectionCard}>
        <div className={styles.dashboardSectionHeader}>
          <h3 className={styles.dashboardSectionTitle}>Recent Transactions</h3>
          <span className={styles.dashboardSectionCount}>click a row for details</span>
        </div>
        {txLoading ? <LoadingState /> : transactions.length === 0 ? (
          <div className={styles.dashboardEmpty}>
            <span className={styles.dashboardEmptyIcon}>💳</span>No transactions yet
          </div>
        ) : (
          <table className={styles.dashboardTable}>
            <thead>
              <tr><th>ID</th><th>User</th><th>Amount</th><th>Country</th><th>Fraud Score</th><th>Status</th><th>Date</th></tr>
            </thead>
            <tbody>
              {transactions.map((t: Transaction) => (
                <tr
                  key={t.transactionId}
                  onClick={() => setSelectedTx(t)}
                  className={styles.dashboardTableRowClickable}
                >
                  <td className={styles.dashboardTableIdCell}>#{t.transactionId}</td>
                  <td
                    className={`${styles.dashboardTableIdCell} ${styles.dashboardUserLink}`}
                    onClick={(e) => { e.stopPropagation(); setSelectedUser(t.userId); }}
                  >
                    #{t.userId}
                  </td>
                  <td><strong>{t.currency.toUpperCase()}</strong> {t.amount.toLocaleString()}</td>
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

      {selectedTx && (
        <TransactionDrawer
          transaction={selectedTx}
          onClose={() => setSelectedTx(null)}
          onViewUser={(userId) => setSelectedUser(userId)}
        />
      )}
      {selectedUser !== null && (
        <UserDetailModal userId={selectedUser} onClose={() => setSelectedUser(null)} />
      )}
    </>
  );
}

export default function DashboardPage() {
  const { email, role } = useAuthStore();
  const firstName = email?.split('@')[0] ?? 'User';
  const isAdmin = role?.toLowerCase() === 'admin';

  return (
    <>
      <div className={styles.dashboardBanner}>
        <div className={styles.dashboardBannerInner}>
          <div>
            <h1 className={styles.dashboardWelcome}>Welcome back, {firstName} 👋</h1>
            <p className={styles.dashboardSubtext}>
              {isAdmin ? 'Platform overview — all systems monitored' : 'Here\'s a summary of your account activity'}
            </p>
          </div>
        </div>
      </div>
      <div className={styles.dashboardBody}>
        {isAdmin ? <AdminDashboard /> : <UserDashboard />}
      </div>
    </>
  );
}
