import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { getTransactions, createTransaction } from '../api/transactionApi';
import { updateTransactionStatus, getUserById, unflagUser } from '../api/adminApi';
import { useAuthStore } from '../store/authstore';
import type { Transaction } from '../types/common.types';
import styles from './TransactionPage.module.css';

const schema = z.object({
  amount: z.coerce.number().min(1, 'Amount must be at least 1'),
  currency: z.string().min(2, 'Enter a valid currency code').max(5),
  country: z.string().min(2, 'Enter a valid Country'),
});

type FormData = z.infer<typeof schema>;

const ALL_STATUSES = ['PENDING', 'APPROVED', 'FLAGGED', 'FAILED'];

function StatusBadge({ status }: { status: string }) {
  const s = status || 'PENDING';
  const cls =
    s === 'FLAGGED' ? styles.txBadgeHigh :
    s === 'PENDING' ? styles.txBadgeMedium :
    s === 'FAILED'  ? styles.txBadgeFailed :
    styles.txBadgeLow;
  return <span className={`${styles.txBadge} ${cls}`}>● {s}</span>;
}

function FraudScoreBar({ score }: { score: number }) {
  const fillCls =
    score >= 60 ? styles.txScoreFillHigh :
    score >= 35 ? styles.txScoreFillMedium :
    styles.txScoreFillLow;
  return (
    <div className={styles.txScoreWrapper}>
      <div className={styles.txScoreBar}>
        <div className={`${styles.txScoreFill} ${fillCls}`} style={{ width: `${score}%` }} />
      </div>
      <span className={styles.txScoreText}>{score}</span>
    </div>
  );
}

function UserDetailModal({ userId, onClose }: { userId: number; onClose: () => void }) {
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
                  <div className={styles.drawerRow}>
                    <span className={styles.drawerLabel}>Name</span>
                    <span className={styles.drawerValue}>{user.name}</span>
                  </div>
                  <div className={styles.drawerRow}>
                    <span className={styles.drawerLabel}>Email</span>
                    <span className={styles.drawerValue}>{user.email}</span>
                  </div>
                  <div className={styles.drawerRow}>
                    <span className={styles.drawerLabel}>Role</span>
                    <span className={styles.drawerValue}>{user.role}</span>
                  </div>
                  <div className={styles.drawerRow}>
                    <span className={styles.drawerLabel}>Joined</span>
                    <span className={styles.drawerValue}>{new Date(user.createdAt).toLocaleDateString()}</span>
                  </div>
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

function TransactionDrawer({
  transaction,
  onClose,
  onStatusChange,
  onViewUser,
  isUpdating,
}: {
  transaction: Transaction;
  onClose: () => void;
  onStatusChange: (id: number, status: string) => void;
  onViewUser: (userId: number) => void;
  isUpdating: boolean;
}) {
  const currentStatus = transaction.status || 'PENDING';

  const { data: user, isLoading: userLoading } = useQuery({
    queryKey: ['adminUser', transaction.userId],
    queryFn: () => getUserById(transaction.userId),
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
                <span className={styles.drawerValue}>
                  <strong>{transaction.amount.toLocaleString()} {transaction.currency.toUpperCase()}</strong>
                </span>
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
                  onClick={() => onStatusChange(transaction.transactionId, s)}
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

export default function TransactionsPage() {
  const [page, setPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [selectedTx, setSelectedTx] = useState<Transaction | null>(null);
  const [selectedUser, setSelectedUser] = useState<number | null>(null);
  const [serverError, setServerError] = useState('');
  const queryClient = useQueryClient();
  const pageSize = 15;
  const role = useAuthStore((s) => s.role);
  const isAdmin = role?.toLowerCase() === 'admin';

  const { data, isLoading } = useQuery({
    queryKey: ['transactions', page],
    queryFn: () => getTransactions(page, pageSize),
  });

  const { mutate: doStatusUpdate, isPending: isUpdating } = useMutation({
    mutationFn: ({ id, status }: { id: number; status: string }) =>
      updateTransactionStatus(id, status),
    onSuccess: (updated) => {
      queryClient.invalidateQueries({ queryKey: ['transactions'] });
      if (updated && selectedTx) setSelectedTx({ ...selectedTx, status: updated.status });
    },
  });

  const transactions = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  const { register, handleSubmit, formState: { errors, isSubmitting }, reset } =
    useForm<FormData>({ resolver: zodResolver(schema) });

  const onSubmit = async (formData: FormData) => {
    setServerError('');
    try {
      await createTransaction(formData.amount, formData.currency.toUpperCase(), formData.country);
      queryClient.invalidateQueries({ queryKey: ['transactions'] });
      setShowModal(false);
      reset();
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message ?? 'Failed to create transaction.';
      setServerError(msg);
    }
  };

  return (
    <>
      <div className={styles.txBanner}>
        <div className={styles.txBannerInner}>
          <div>
            <h1 className={styles.txTitle}>Transactions</h1>
            <p className={styles.txSubtext}>
              {isAdmin ? 'All transactions across the platform' : 'Your full transaction history with fraud scores'}
            </p>
          </div>
          {!isAdmin && (
            <button className={styles.txAddBtn} onClick={() => setShowModal(true)}>
              + New Transaction
            </button>
          )}
        </div>
      </div>

      <div className={styles.txBody}>
        <div className={styles.txCard}>
          <div className={styles.txCardHeader}>
            <h3 className={styles.txCardTitle}>All Transactions</h3>
            <span className={styles.txCardCount}>{data?.totalRecords ?? 0} total</span>
          </div>

          {isLoading ? (
            <div className={styles.txLoading}>
              <div className={styles.txSpinner} />
              Loading...
            </div>
          ) : transactions.length === 0 ? (
            <div className={styles.txEmpty}>
              <span className={styles.txEmptyIcon}>💳</span>
              No transactions yet
            </div>
          ) : (
            <table className={styles.txTable}>
              <thead>
                <tr>
                  <th>ID</th>
                  {isAdmin && <th>User</th>}
                  <th>Amount</th>
                  <th>Currency</th>
                  <th>Country</th>
                  <th>Fraud Score</th>
                  <th>Status</th>
                  <th>Date</th>
                </tr>
              </thead>
              <tbody>
                {transactions.map((t: Transaction) => (
                  <tr
                    key={t.transactionId}
                    onClick={() => isAdmin && setSelectedTx(t)}
                    className={isAdmin ? styles.txRowClickable : ''}
                  >
                    <td className={styles.txIdCell}>#{t.transactionId}</td>
                    {isAdmin && (
                      <td
                        className={`${styles.txIdCell} ${styles.txUserLink}`}
                        onClick={(e) => { e.stopPropagation(); setSelectedUser(t.userId); }}
                      >
                        #{t.userId}
                      </td>
                    )}
                    <td><strong>{t.amount.toLocaleString()}</strong></td>
                    <td>{t.currency}</td>
                    <td>{t.country}</td>
                    <td><FraudScoreBar score={t.fraudScore} /></td>
                    <td><StatusBadge status={t.status} /></td>
                    <td>{new Date(t.transactionTime).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {totalPages > 1 && (
            <div className={styles.txPagination}>
              <button className={styles.txPageBtn} onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1}>← Prev</button>
              <span className={styles.txPageInfo}>Page {page} of {totalPages}</span>
              <button className={styles.txPageBtn} onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page === totalPages}>Next →</button>
            </div>
          )}
        </div>
      </div>

      {selectedTx && isAdmin && (
        <TransactionDrawer
          transaction={selectedTx}
          onClose={() => setSelectedTx(null)}
          onStatusChange={(id, status) => doStatusUpdate({ id, status })}
          onViewUser={(userId) => setSelectedUser(userId)}
          isUpdating={isUpdating}
        />
      )}

      {selectedUser !== null && (
        <UserDetailModal
          userId={selectedUser}
          onClose={() => setSelectedUser(null)}
        />
      )}

      {showModal && (
        <div className={styles.txModalOverlay} onClick={() => setShowModal(false)}>
          <div className={styles.txModal} onClick={(e) => e.stopPropagation()}>
            <div className={styles.txModalHeader}>
              <h2 className={styles.txModalTitle}>New Transaction</h2>
              <button className={styles.txModalClose} onClick={() => setShowModal(false)}>✕</button>
            </div>
            {serverError && <div className={styles.txModalError}>{serverError}</div>}
            <form onSubmit={handleSubmit(onSubmit)} className={styles.txModalForm} noValidate>
              <div className={styles.txModalField}>
                <label className={styles.txModalLabel}>Amount</label>
                <input {...register('amount')} type="number" className={`${styles.txModalInput} ${errors.amount ? styles.txModalInputError : ''}`} placeholder="e.g. 1500" />
                {errors.amount && <span className={styles.txModalFieldError}>{errors.amount.message}</span>}
              </div>
              <div className={styles.txModalField}>
                <label className={styles.txModalLabel}>Currency</label>
                <input {...register('currency')} className={`${styles.txModalInput} ${errors.currency ? styles.txModalInputError : ''}`} placeholder="e.g. CAD, USD" />
                {errors.currency && <span className={styles.txModalFieldError}>{errors.currency.message}</span>}
              </div>
              <div className={styles.txModalField}>
                <label className={styles.txModalLabel}>Country</label>
                <input {...register('country')} className={`${styles.txModalInput} ${errors.country ? styles.txModalInputError : ''}`} placeholder="e.g. Canada" />
                {errors.country && <span className={styles.txModalFieldError}>{errors.country.message}</span>}
              </div>
              <div className={styles.txModalActions}>
                <button type="button" className={styles.txModalCancelBtn} onClick={() => setShowModal(false)}>Cancel</button>
                <button type="submit" className={styles.txModalSubmitBtn} disabled={isSubmitting}>{isSubmitting ? 'Submitting...' : 'Submit'}</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
