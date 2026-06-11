import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getAdminUsers, unflagUser } from '../api/adminApi';
import type { User } from '../types/common.types';
import styles from './AdminUserPage.module.css';

function RoleBadge({ role }: { role: string }) {
  const cls = role === 'Admin' ? styles.badgeAdmin : styles.badgeUser;
  return <span className={`${styles.badge} ${cls}`}>{role}</span>;
}

function FlagBadge({ isFlagged }: { isFlagged: boolean }) {
  return isFlagged
    ? <span className={`${styles.badge} ${styles.badgeFlagged}`}>● Flagged</span>
    : <span className={`${styles.badge} ${styles.badgeClean}`}>● Clean</span>;
}

export default function AdminUsersPage() {
  const [page, setPage] = useState(1);
  const pageSize = 15;
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['adminUsers', page],
    queryFn: () => getAdminUsers(page, pageSize),
  });

  const { mutate: doUnflag, isPending } = useMutation({
    mutationFn: unflagUser,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['adminUsers'] }),
  });

  const users = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  return (
    <>
      <div className={styles.banner}>
        <div className={styles.bannerInner}>
          <div>
            <h1 className={styles.bannerTitle}>User Management</h1>
            <p className={styles.bannerSub}>View and manage all registered users</p>
          </div>
          <span className={styles.bannerStat}>{data?.totalRecords ?? 0} total users</span>
        </div>
      </div>

      <div className={styles.body}>
        <div className={styles.card}>
          <div className={styles.cardHeader}>
            <h3 className={styles.cardTitle}>All Users</h3>
            <span className={styles.cardCount}>{data?.totalRecords ?? 0} records</span>
          </div>

          {isLoading ? (
            <div className={styles.loading}>
              <div className={styles.spinner} />
              Loading...
            </div>
          ) : users.length === 0 ? (
            <div className={styles.empty}>
              <span className={styles.emptyIcon}>👥</span>
              No users found
            </div>
          ) : (
            <table className={styles.table}>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Role</th>
                  <th>Status</th>
                  <th>Joined</th>
                  <th>Action</th>
                </tr>
              </thead>
              <tbody>
                {users.map((u: User) => (
                  <tr key={u.userId}>
                    <td className={styles.idCell}>#{u.userId}</td>
                    <td>{u.name}</td>
                    <td className={styles.emailCell}>{u.email}</td>
                    <td><RoleBadge role={u.role} /></td>
                    <td><FlagBadge isFlagged={u.isFlagged} /></td>
                    <td>{new Date(u.createdAt).toLocaleDateString()}</td>
                    <td>
                      {u.isFlagged && (
                        <button
                          className={styles.unflagBtn}
                          disabled={isPending}
                          onClick={() => doUnflag(u.userId)}
                        >
                          Unflag
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          {totalPages > 1 && (
            <div className={styles.pagination}>
              <button
                className={styles.pageBtn}
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
              >
                ← Prev
              </button>
              <span className={styles.pageInfo}>Page {page} of {totalPages}</span>
              <button
                className={styles.pageBtn}
                onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                disabled={page === totalPages}
              >
                Next →
              </button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
