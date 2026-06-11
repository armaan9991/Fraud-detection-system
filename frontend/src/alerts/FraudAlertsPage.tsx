import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getFraudAlerts } from '../api/fraudAlertApi';
import { getTransactionById } from '../api/transactionApi';
import { useAuthStore } from '../store/authstore';
import { TransactionDrawer, UserDetailModal } from '../components/TransactionDrawer';
import type { FraudAlert, Transaction } from '../types/common.types';
import styles from './FraudAlertsPage.module.css';

function RiskBadge({ level }: { level: string }) {
  const cls =
    level === 'HIGH' ? styles.alertBadgeHigh :
    level === 'MEDIUM' ? styles.alertBadgeMedium :
    styles.alertBadgeLow;
  return <span className={`${styles.alertBadge} ${cls}`}>● {level}</span>;
}

export default function AlertsPage() {
  const [page, setPage] = useState(1);
  const [selectedTxId, setSelectedTxId] = useState<number | null>(null);
  const [selectedUser, setSelectedUser] = useState<number | null>(null);
  const pageSize = 15;
  const role = useAuthStore((s) => s.role);
  const isAdmin = role?.toLowerCase() === 'admin';

  const { data, isLoading } = useQuery({
    queryKey: ['fraudAlerts', page],
    queryFn: () => getFraudAlerts(page, pageSize),
  });

  const { data: selectedTx } = useQuery<Transaction>({
    queryKey: ['transaction', selectedTxId],
    queryFn: () => getTransactionById(selectedTxId!),
    enabled: selectedTxId !== null,
  });

  const alerts = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <>
      <div className={styles.alertBanner}>
        <div className={styles.alertBannerInner}>
          <div>
            <h1 className={styles.alertTitle}>Fraud Alerts</h1>
            <p className={styles.alertSubtext}>Click any alert to view transaction and user details</p>
          </div>
          <div className={styles.alertBannerStats}>
            <span className={styles.alertBannerStat}>{data?.totalRecords ?? 0} total alerts</span>
          </div>
        </div>
      </div>

      <div className={styles.alertBody}>
        <div className={styles.alertCard}>
          <div className={styles.alertCardHeader}>
            <h3 className={styles.alertCardTitle}>All Alerts</h3>
            <span className={styles.alertCardCount}>{data?.totalRecords ?? 0} records</span>
          </div>

          {isLoading ? (
            <div className={styles.alertLoading}>
              <div className={styles.alertSpinner} />
              Loading...
            </div>
          ) : alerts.length === 0 ? (
            <div className={styles.alertEmpty}>
              <span className={styles.alertEmptyIcon}>🚨</span>
              No fraud alerts found
            </div>
          ) : (
            <table className={styles.alertTable}>
              <thead>
                <tr>
                  <th>Alert ID</th>
                  <th>Transaction ID</th>
                  <th>Risk Level</th>
                  <th>Reason</th>
                  <th>Date</th>
                </tr>
              </thead>
              <tbody>
                {alerts.map((a: FraudAlert) => (
                  <tr
                    key={a.fraudAlertId}
                    onClick={() => setSelectedTxId(a.transactionId)}
                    className={styles.alertRowClickable}
                  >
                    <td className={styles.alertIdCell}>#{a.fraudAlertId}</td>
                    <td className={styles.alertIdCell}>#{a.transactionId}</td>
                    <td><RiskBadge level={a.riskLevel} /></td>
                    <td className={styles.alertReasonCell}>{a.reason}</td>
                    <td>{new Date(a.createdAt).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {totalPages > 1 && (
            <div className={styles.alertPagination}>
              <button className={styles.alertPageBtn} onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1}>← Prev</button>
              <span className={styles.alertPageInfo}>Page {page} of {totalPages}</span>
              <button className={styles.alertPageBtn} onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page === totalPages}>Next →</button>
            </div>
          )}
        </div>
      </div>

      {selectedTx && (
        <TransactionDrawer
          transaction={selectedTx}
          onClose={() => setSelectedTxId(null)}
          onViewUser={(userId) => setSelectedUser(userId)}
          isAdmin={isAdmin}
        />
      )}
      {selectedUser !== null && (
        <UserDetailModal userId={selectedUser} onClose={() => setSelectedUser(null)} />
      )}
    </>
  );
}
