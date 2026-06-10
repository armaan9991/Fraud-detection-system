import { useNavigate,Link } from "react-router-dom";
import { useAuthStore } from "../store/authstore";
import { useState } from "react";
import { z } from "zod";
import { login } from '../api/authApi';
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import styles from './LoginPage.module.css';

const schema = z.object({
  email: z.string().email('Enter a valid email address'),
  password: z.string().min(1, 'Password is required'),
});

type LoginForm = z.infer<typeof schema>;     // here is creates TS type from validation schema

export default function LoginPage() {
  const navigate = useNavigate();
  const setAuth = useAuthStore((state) => state.setAuth);
  const [serverError, setServerError] = useState<string | null>(null);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<LoginForm>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: LoginForm) => {
    setServerError(null);
    try {
      const result = await login(data.email, data.password);
      setAuth(result.accessToken, result.refreshToken, result.email, result.role.toLowerCase());
      navigate('/dashboard');
    } catch {
      setServerError('Invalid email or password. Please try again.');
    }
  };

  return (
    <div className={styles.page}>

      {/* Left Panel */}
      <div className={styles.leftPanel}>
        <div className={styles.brandArea}>
          <div className={styles.brandLogo}>
            <div>
              <div className={styles.brandName}>FraudDetect</div>
              <div className={styles.brandTagline}>Enterprise Security Platform</div>
            </div>
          </div>

          <h2 className={styles.leftHeading}>
            Protecting every transaction in real time
          </h2>
          <p className={styles.leftSubtext}>
            AI-powered fraud detection that monitors, scores, and alerts — 
            so your team can act before damage is done.
          </p>

          <div className={styles.featureList}>
            <div className={styles.featureItem}>
              <span className={styles.featureDot} />
              <span className={styles.featureText}>Real-time ML scoring on every transaction</span>
            </div>
            <div className={styles.featureItem}>
              <span className={styles.featureDot} />
              <span className={styles.featureText}>Automatic alerts for high-risk activity</span>
            </div>
            <div className={styles.featureItem}>
              <span className={styles.featureDot} />
              <span className={styles.featureText}>Role-based access for teams and admins</span>
            </div>
            <div className={styles.featureItem}>
              <span className={styles.featureDot} />
              <span className={styles.featureText}>Full audit trail on every action</span>
            </div>
          </div>
        </div>

        <div className={styles.leftFooter}>
          <span className={styles.secureTag}>256-bit encrypted connection</span>
        </div>
      </div>

      <div className={styles.rightPanel}>
        <div className={styles.formCard}>
          <h1 className={styles.formHeading}>Sign in</h1>
          <p className={styles.formSubheading}>Enter your credentials to access the platform</p>

          <form onSubmit={handleSubmit(onSubmit)} className={styles.form}>

            <div className={styles.field}>
              <label className={styles.label}>Email address</label>
              <input
                {...register('email')}
                type="email"
                placeholder="you@company.com"
                className={`${styles.input} ${errors.email ? styles.inputError : ''}`}
              />
              {errors.email && <p className={styles.fieldError}>{errors.email.message}</p>}
            </div>

            <div className={styles.field}>
              <label className={styles.label}>Password</label>
              <input
                {...register('password')}
                type="password"
                placeholder="••••••••"
                className={`${styles.input} ${errors.password ? styles.inputError : ''}`}
              />
              {errors.password && <p className={styles.fieldError}>{errors.password.message}</p>}
            </div>

            {serverError && (
              <p className={styles.serverError}>⚠ {serverError}</p>
            )}

            <button
              type="submit"
              disabled={isSubmitting}
              className={styles.submitButton}
            >
              {isSubmitting ? 'Signing in...' : 'Sign in'}
            </button>
            <p className={styles.loginFooter}>
              Dont have account!{' '}
              <Link to="/register" className={styles.loginFooterLink}>Sign up</Link>
            </p>

          </form>

          <div className={styles.formFooter}>
            Secured by FraudDetect — 2026
          </div>
        </div>
      </div>

    </div>
  );
}
