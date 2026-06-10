import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getUserById, unflagUser, updateTransactionStatus } from '../api/adminApi';
import type { Transaction } from '../types/common.types';
import styles from './TransactionDrawer.module.css';

const ALL_STATUSES = ['PENDING', 'APPROVED', 'FLAGGED', 'FAILED'];

export function StatusBadge({ status }: { status: string }) {
  const s = status || 'PENDING';
  const cls =
    s === 'FLAGGED' ? styles.badgeHigh :
    s === 'PENDING' ? styles.badgeMedium :
    s === 'FAILED'  ? styles.badgeFailed :
    styles.badgeLow;
  return <span className={`${styles.badge} ${cls}`}>● {s}</span>;
}

export function FraudScoreBar({ score }: { score: number }) {
  const fillCls =
    score >= 60 ? styles.scoreFillHigh :
    score >= 35 ? styles.scoreFillMedium :
    styles.scoreFillLow;
  return (
    <div className={styles.scoreWrapper}>
      <div className={styles.scoreBar}>
        <div className={`${styles.scoreFill} ${fillCls}`} style={{ width: `${score}%` }} />
      </div>
      <span className={styles.scoreText}>{score}</span>
    </div>
  );
}

export function UserDetailModal({ userId, onClose }: { userId: number; onClose: () => void }) {
  const queryClient = useQueryClient();

  const { data: user, isLoading } = useQuery({
    queryKey: ['adminUser', userId],
    queryFn: () => getUserById(userId),
  });

  const { mutate: doUnflag, isPending: unflagging } = useMutation({
    mutationFn: () => unflagUser(userId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['adminUser', userId] }),
  });

  return (
    <div className={styles.userModalOverlay} onClick={onClose}>
      <div className={styles.userModal} onClick={(e) => e.stopPropagation()}>
        <div className={styles.drawerHeader}>
          <div>
            <span className={styles.drawerHeaderSub}>User Profile</span>
            <h2 className={styles.drawerHeaderTitle}>#{userId}</h2>
          </div>
          <button className={styles.drawerClose} onClick={onClose}>✕</button>
        </div>
        <div className={styles.drawerBody}>
          {isLoading ? (
            <p className={styles.drawerMuted}>Loading...</p>
          ) : user ? (
            <>
              <div className={styles.drawerSection}>
                <h3 className={styles.drawerSectionTitle}>Info</h3>
                <div className={styles.drawerGrid}>
                  <div className={styles.drawerRow}><span className={styles.drawerLabel}>Name</span><span className={styles.drawerValue}>{user.name}</span></div>
                  <div className={styles.drawerRow}><span className={styles.drawerLabel}>Email</span><span className={styles.drawerValue}>{user.email}</span></div>
                  <div className={styles.drawerRow}><span className={styles.drawerLabel}>Role</span><span className={styles.drawerValue}>{user.role}</span></div>
                  <div className={styles.drawerRow}><span className={styles.drawerLabel}>Joined</span><span className={styles.drawerValue}>{new Date(user.createdAt).toLocaleDateString()}</span></div>
                </div>
              </div>
              <div className={styles.drawerSection}>
                <h3 className={styles.drawerSectionTitle}>Flag Status</h3>
                {user.isFlagged ? (
                  <>
                    <p className={styles.flaggedYes}>● Flagged — {user.flagReason || 'No reason provided'}</p>
                    <button className={styles.unflagBtn} disabled={unflagging} onClick={() => doUnflag()}>
                      {unflagging ? 'Unflagging...' : 'Unflag User'}
                    </button>
                  </>
                ) : (
                  <p className={styles.flaggedNo}>● Not flagged</p>
                )}
              </div>
            </>
          ) : (
            <p className={styles.drawerMuted}>User not found</p>
          )}
        </div>
      </div>
    </div>
  );
}

export function TransactionDrawer({
  transaction,
  onClose,
  onViewUser,
}: {
  transaction: Transaction;
  onClose: () => void;
  onViewUser: (userId: number) => void;
}) {
  const queryClient = useQueryClient();
  const currentStatus = transaction.status || 'PENDING';

  const { data: user, isLoading: userLoading } = useQuery({
    queryKey: ['adminUser', transaction.userId],
    queryFn: () => getUserById(transaction.userId),
  });

  const { mutate: doStatusUpdate, isPending: isUpdating } = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      updateTransactionStatus(id, status),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['transactions'] }),
  });

  return (
    <div className={styles.drawerOverlay} onClick={onClose}>
      <div className={styles.drawer} onClick={(e) => e.stopPropagation()}>
        <div className={styles.drawerHeader}>
          <div>
            <span className={styles.drawerHeaderSub}>Transaction</span>
            <h2 className={styles.drawerHeaderTitle}>#{transaction.transactionId}</h2>
          </div>
          <button className={styles.drawerClose} onClick={onClose}>✕</button>
        </div>
        <div className={styles.drawerBody}>

          <div className={styles.drawerSection}>
            <h3 className={styles.drawerSectionTitle}>Details</h3>
            <div className={styles.drawerGrid}>
              <div className={styles.drawerRow}>
                <span className={styles.drawerLabel}>Amount</span>
                <span className={styles.drawerValue}><strong>{transaction.amount.toLocaleString()} {transaction.currency.toUpperCase()}</strong></span>
              </div>
              <div className={styles.drawerRow}>
                <span className={styles.drawerLabel}>Country</span>
                <span className={styles.drawerValue}>{transaction.country}</span>
              </div>
              <div className={styles.drawerRow}>
                <span className={styles.drawerLabel}>Date</span>
                <span className={styles.drawerValue}>{new Date(transaction.transactionTime).toLocaleString()}</span>
              </div>
              <div className={styles.drawerRow}>
                <span className={styles.drawerLabel}>Fraud Score</span>
                <span className={styles.drawerValue}><FraudScoreBar score={transaction.fraudScore} /></span>
              </div>
              <div className={styles.drawerRow}>
                <span className={styles.drawerLabel}>Status</span>
                <span className={styles.drawerValue}><StatusBadge status={currentStatus} /></span>
              </div>
            </div>
          </div>

          <div className={styles.drawerSection}>
            <h3 className={styles.drawerSectionTitle}>Change Status</h3>
            <div className={styles.statusBtnGroup}>
              {ALL_STATUSES.map((s) => (
                <button
                  key={s}
                  disabled={isUpdating || s === currentStatus}
                  onClick={() => doStatusUpdate({ id: transaction.transactionId, status: s })}
                  className={`${styles.statusBtn} ${
                    s === currentStatus ? styles.statusBtnActive :
                    s === 'FLAGGED'     ? styles.statusBtnFlag :
                    s === 'APPROVED'    ? styles.statusBtnApprove :
                    s === 'FAILED'      ? styles.statusBtnFailed :
                    styles.statusBtnPending
                  }`}
                >
                  {s}
                </button>
              ))}
            </div>
          </div>

          <div className={styles.drawerSection}>
            <h3 className={styles.drawerSectionTitle}>User</h3>
            {userLoading ? (
              <p className={styles.drawerMuted}>Loading user...</p>
            ) : user ? (
              <button className={styles.userCard} onClick={() => onViewUser(transaction.userId)}>
                <div className={styles.userCardAvatar}>{user.name.slice(0, 2).toUpperCase()}</div>
                <div className={styles.userCardInfo}>
                  <span className={styles.userCardName}>{user.name}</span>
                  <span className={styles.userCardId}>#{transaction.userId}</span>
                </div>
                <span className={user.isFlagged ? styles.flaggedYes : styles.flaggedNo}>
                  {user.isFlagged ? '● Flagged' : '● Clean'}
                </span>
                <span className={styles.userCardArrow}>›</span>
              </button>
            ) : (
              <p className={styles.drawerMuted}>User #{transaction.userId}</p>
            )}
          </div>

        </div>
      </div>
    </div>
  );
}
