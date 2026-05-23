import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useNavigate, Link } from 'react-router-dom';
import { CreateUserAsync } from '../api/userapi';
import styles from '../auth/LoginPage.module.css';

const schema = z.object({
  name: z.string().min(2, 'Name must be at least 2 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(8, 'Password must be at least 8 characters'),
  confirmPassword: z.string(),
}).refine((d) => d.password === d.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
});

type FormData = z.infer<typeof schema>;

export default function RegisterPage() {
  const navigate = useNavigate();
  const [serverError, setServerError] = useState('');

  const { register, handleSubmit, formState: { errors, isSubmitting } } =
    useForm<FormData>({ resolver: zodResolver(schema) });

  const onSubmit = async (data: FormData) => {
    setServerError('');
    try {
      await CreateUserAsync(data.name, data.email, 'User', data.password);
      navigate('/login');
    } catch (err: unknown) {
      const msg = (err as { response?: { data?: { message?: string } } })
        ?.response?.data?.message ?? 'Registration failed. Please try again.';
      setServerError(msg);
    }
  };

  return (
    <div className={styles.page}>
      <div className={styles.leftPanel}>
        <div className={styles.brandArea}>
          <div className={styles.brandLogo}>
            <div className={styles.brandIcon}>🛡️</div>
            <div>
              <div className={styles.brandName}>FraudDetect</div>
              <div className={styles.brandTagline}>Powered by AI</div>
            </div>
          </div>
          <h2 className={styles.leftHeading}>Secure. Smart. Real-time.</h2>
          <p className={styles.leftSubtext}>
            Join the platform protecting transactions with AI-powered fraud detection.
          </p>
          <div className={styles.featureList}>
            <div className={styles.featureItem}>
              <div className={styles.featureDot} />
              <span className={styles.featureText}>Real-time fraud scoring</span>
            </div>
            <div className={styles.featureItem}>
              <div className={styles.featureDot} />
              <span className={styles.featureText}>Instant fraud alerts</span>
            </div>
            <div className={styles.featureItem}>
              <div className={styles.featureDot} />
              <span className={styles.featureText}>Bank-grade security</span>
            </div>
          </div>
        </div>
        <div className={styles.leftFooter}>
          <span className={styles.secureTag}>🔒 256-bit encrypted</span>
        </div>
      </div>

      <div className={styles.rightPanel}>
        <div className={styles.formCard}>
          <h1 className={styles.formHeading}>Create account</h1>
          <p className={styles.formSubheading}>Sign up to get started</p>

          {serverError && <div className={styles.serverError}>{serverError}</div>}

          <form onSubmit={handleSubmit(onSubmit)} className={styles.form} noValidate>
            <div className={styles.field}>
              <label className={styles.label}>Full Name</label>
              <input
                {...register('name')}
                className={`${styles.input} ${errors.name ? styles.inputError : ''}`}
                placeholder="John Doe"
              />
              {errors.name && <span className={styles.fieldError}>{errors.name.message}</span>}
            </div>

            <div className={styles.field}>
              <label className={styles.label}>Email Address</label>
              <input
                {...register('email')}
                type="email"
                className={`${styles.input} ${errors.email ? styles.inputError : ''}`}
                placeholder="you@example.com"
              />
              {errors.email && <span className={styles.fieldError}>{errors.email.message}</span>}
            </div>

            <div className={styles.field}>
              <label className={styles.label}>Password</label>
              <input
                {...register('password')}
                type="password"
                className={`${styles.input} ${errors.password ? styles.inputError : ''}`}
                placeholder="Min. 8 characters"
              />
              {errors.password && <span className={styles.fieldError}>{errors.password.message}</span>}
            </div>

            <div className={styles.field}>
              <label className={styles.label}>Confirm Password</label>
              <input
                {...register('confirmPassword')}
                type="password"
                className={`${styles.input} ${errors.confirmPassword ? styles.inputError : ''}`}
                placeholder="Repeat your password"
              />
              {errors.confirmPassword && <span className={styles.fieldError}>{errors.confirmPassword.message}</span>}
            </div>

            <button type="submit" className={styles.submitButton} disabled={isSubmitting}>
              {isSubmitting ? 'Creating account...' : 'Create Account'}
            </button>
          </form>

          <p className={styles.loginFooter}>
            Already have an account?{' '}
            <Link to="/login" className={styles.loginFooterLink}>Sign in</Link>
          </p>
        </div>
      </div>
    </div>
  );
}
