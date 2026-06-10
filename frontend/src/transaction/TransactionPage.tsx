import { useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { getTransactions, createTransaction } from '../api/transactionApi';
import { useAuthStore } from '../store/authstore';
import type { Transaction } from '../types/common.types';
import styles from './TransactionPage.module.css';
import { TransactionDrawer, UserDetailModal, StatusBadge, FraudScoreBar } from '../components/TransactionDrawer';

const schema = z.object({
  amount: z.coerce.number().min(1, 'Amount must be at least 1'),
  currency: z.string().min(2, 'Enter a valid currency code').max(5),
  country: z.string().min(2, 'Enter a valid Country'),
});

type FormData = z.infer<typeof schema>;

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
          onViewUser={(userId) => setSelectedUser(userId)}
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
