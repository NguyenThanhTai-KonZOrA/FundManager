import React, { useState, useEffect } from "react";
import {
    TextField,
    Button,
    Typography,
    Box,
    Alert,
    CircularProgress,
    InputAdornment,
    IconButton,
    Divider,
    Chip,
} from "@mui/material";
import { useTheme } from '@mui/material/styles';
import { useNavigate, useLocation } from "react-router-dom";
import axios from "axios";
import { authAdminService as loginApi } from "../services/authService";
import { useAuth } from "../contexts/AuthContext";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import PersonOutlineIcon from "@mui/icons-material/PersonOutline";
import Visibility from "@mui/icons-material/Visibility";
import VisibilityOff from "@mui/icons-material/VisibilityOff";
import AssignmentTurnedInOutlinedIcon from "@mui/icons-material/AssignmentTurnedInOutlined";
import DrawOutlinedIcon from "@mui/icons-material/DrawOutlined";
import BarChartOutlinedIcon from "@mui/icons-material/BarChartOutlined";
import WifiOutlinedIcon from "@mui/icons-material/WifiOutlined";
import { UserRole } from "../constants/roles";

const BRAND_DARK = "#1a2f32";
const BRAND_TEAL = "#274549";
const BRAND_CYAN = "#0d93c9";
const GOLD = "#c9a84c";
const GOLD_LIGHT = "#e8c96a";

interface LoginLocationState {
    from?: {
        pathname?: string;
    };
}

interface CasinoFloater {
    symbol: string;
    top: string;
    left?: string;
    right?: string;
    size: string;
    duration: string;
    delay: string;
    opacity: number;
}

// Casino floating background symbols
const casinoFloaters: CasinoFloater[] = [
    { symbol: "♠", top: "6%", left: "8%", size: "25rem", duration: "14s", delay: "0s", opacity: 0.13 },
    { symbol: "♥", top: "3%", right: "9%", size: "10.8rem", duration: "20s", delay: "2s", opacity: 0.10 },
    { symbol: "♦", top: "42%", left: "5%", size: "10.5rem", duration: "25s", delay: "4s", opacity: 0.09 },
    { symbol: "♣", top: "70%", right: "12%", size: "2.2rem", duration: "17s", delay: "1s", opacity: 0.12 },
    { symbol: "♠", top: "55%", left: "78%", size: "1.6rem", duration: "22s", delay: "6s", opacity: 0.08 },
    { symbol: "♥", top: "83%", left: "28%", size: "20.8rem", duration: "28s", delay: "2s", opacity: 0.07 },
    { symbol: "♦", top: "12%", left: "52%", size: "1.4rem", duration: "18s", delay: "5s", opacity: 0.10 },
    { symbol: "♣", top: "34%", right: "6%", size: "20.6rem", duration: "32s", delay: "7s", opacity: 0.08 },
    { symbol: "🎲", top: "60%", left: "15%", size: "15.8rem", duration: "24s", delay: "0s", opacity: 0.09 },
    { symbol: "🪙", top: "90%", right: "20%", size: "5rem", duration: "19s", delay: "11s", opacity: 0.08 },
    { symbol: "🃏", top: "28%", left: "40%", size: "8.6rem", duration: "0s", delay: "80s", opacity: 0.07 },
    { symbol: "💎", top: "56%", left: "55%", size: "10.4rem", duration: "21s", delay: "130s", opacity: 0.09 },
];

// Documenting the features of the platform
const documentsFloaters: CasinoFloater[] = [
    { symbol: "📄", top: "10%", left: "15%", size: "12rem", duration: "20s", delay: "0s", opacity: 0.12 },
    { symbol: "📝", top: "30%", right: "10%", size: "8rem", duration: "25s", delay: "5s", opacity: 0.10 },
    { symbol: "📑", top: "50%", left: "20%", size: "10rem", duration: "30s", delay: "10s", opacity: 0.08 },
    { symbol: "🖋️", top: "70%", right: "15%", size: "6rem", duration: "35s", delay: "15s", opacity: 0.09 },
    { symbol: "📜", top: "75%", left: "5%", size: "14rem", duration: "40s", delay: "20s", opacity: 0.11 },
    { symbol: "🗂️", top: "20%", right: "30%", size: "9rem", duration: "22s", delay: "25s", opacity: 0.07 },
    { symbol: "📋", top: "40%", left: "35%", size: "11rem", duration: "28s", delay: "30s", opacity: 0.10 },
    { symbol: "🗃️", top: "60%", right: "25%", size: "7rem", duration: "32s", delay: "35s", opacity: 0.08 },
    { symbol: "📇", top: "75%", left: "40%", size: "13rem", duration: "38s", delay: "40s", opacity: 0.09 },
    { symbol: "🗄️", top: "90%", right: "20%", size: "10rem", duration: "45s", delay: "45s", opacity: 0.07 },
    { symbol: "📎", top: "40%", right: "40%", size: "4rem", duration: "24s", delay: "55s", opacity: 0.09 },
    { symbol: "🖇️", top: "55%", left: "60%", size: "6rem", duration: "30s", delay: "60s", opacity: 0.07 },
];

const features = [
    {
        icon: <AssignmentTurnedInOutlinedIcon sx={{ fontSize: 22, color: GOLD }} />,
        label: "Customer Signed Consent",
        desc: "Track registered & pending hotel guests in real-time",
    },
    {
        icon: <DrawOutlinedIcon sx={{ fontSize: 22, color: GOLD }} />,
        label: "Digital Signature",
        desc: "Capture & archive signed documents via iPad",
    },
    {
        icon: <BarChartOutlinedIcon sx={{ fontSize: 22, color: GOLD }} />,
        label: "Analytics & Reports",
        desc: "Comprehensive membership & registration reports",
    },
    {
        icon: <WifiOutlinedIcon sx={{ fontSize: 22, color: GOLD }} />,
        label: "Live Notifications",
        desc: "Real-time SignalR messaging across all devices",
    },
];

const Login: React.FC = () => {
    const [username, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const [showPwd, setShowPwd] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const { login, token, role } = useAuth();
    const muiTheme = useTheme();

    useEffect(() => {
        if (token && !loading) {
            const state = location.state as LoginLocationState | null;
            const from = state?.from?.pathname;
            if (from && from !== "/login") {
                navigate(from, { replace: true });
            } else {
                if (role === UserRole.ADMIN) {
                    navigate("/admin-documents-signed", { replace: true });
                } else if (role === UserRole.USER || role === UserRole.TSO || role === UserRole.OUTLET_STAFF) {
                    navigate("/admin-documents-signed", { replace: true });
                } else {
                    navigate("/admin-documents-signed", { replace: true });
                }
            }
        }
    }, [token, navigate, location.state, loading, role]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        setLoading(true);

        if (!username.trim() || !password.trim()) {
            setError("Please enter both username and password");
            setLoading(false);
            return;
        }

        try {
            const response = await loginApi.loginAsync({ userName: username, password });

            if (response && response.token && response.refreshToken && response.tokenExpiration) {
                login(response.userName, response.token, response.refreshToken, response.tokenExpiration);
                setLoading(false);
            } else {
                setError("Invalid response from server");
                setLoading(false);
            }
        } catch (err: unknown) {
            console.error("Sign in error:", err);

            let errorMessage = "Sign in failed";
            if (axios.isAxiosError(err) && err.response?.data && typeof err.response.data === "object" && 'message' in err.response.data && typeof err.response.data.message === 'string') {
                errorMessage = err.response.data.message;
            } else if (axios.isAxiosError(err) && err.response?.data) {
                errorMessage = typeof err.response.data === "string"
                    ? err.response.data
                    : "Invalid username or password";
            } else if (err instanceof Error && err.message) {
                errorMessage = err.message;
            }

            setError(errorMessage);
            setLoading(false);
        }
    };

    return (
        <Box
            sx={{
                position: "fixed",
                inset: 0,
                display: "flex",
                minHeight: "100vh",
                minWidth: "100vw",
                overflow: "hidden",
                fontFamily: '"Inter", "Roboto", sans-serif',
            }}
        >
            {/* ── Left branding panel ── */}
            <Box
                sx={{
                    display: { xs: "none", md: "flex" },
                    flexDirection: "column",
                    justifyContent: "space-between",
                    width: "65%",
                    position: "relative",
                    overflow: "hidden",
                    background: `linear-gradient(160deg, ${muiTheme.palette.primary.dark} 0%, ${muiTheme.palette.primary.main} 55%, ${muiTheme.palette.primary.light} 100%)`,
                    p: { md: 5, lg: 7 },
                }}
            >
                {/* decorative circles */}
                <Box sx={{
                    position: "absolute", top: -80, right: -80,
                    width: 340, height: 340, borderRadius: "50%",
                    background: `radial-gradient(circle, ${muiTheme.palette.secondary.main}22 0%, transparent 70%)`,
                    pointerEvents: "none",
                }} />
                <Box sx={{
                    position: "absolute", bottom: -120, left: -60,
                    width: 420, height: 420, borderRadius: "50%",
                    background: `radial-gradient(circle, ${GOLD}18 0%, transparent 70%)`,
                    pointerEvents: "none",
                }} />

                {/* floating casino symbols */}
                {documentsFloaters.map((f, i) => (
                    <Box
                        key={i}
                        sx={{
                            position: "absolute",
                            top: f.top,
                            left: f.left,
                            right: f.right,
                            fontSize: f.size,
                            opacity: f.opacity,
                            pointerEvents: "none",
                            userSelect: "none",
                            lineHeight: 1,
                            filter: "drop-shadow(0 0 6px rgba(201,168,76,0.4))",
                            "@keyframes casinoSpin": {
                                "0%": { transform: "rotate(0deg) scale(1)" },
                                "50%": { transform: "rotate(180deg) scale(1.12)" },
                                "100%": { transform: "rotate(360deg) scale(1)" },
                            },
                            animation: `casinoSpin ${f.duration} linear ${f.delay} infinite`,
                        }}
                    >
                        {f.symbol}
                    </Box>
                ))}
                {/* thin gold top border */}
                <Box sx={{
                    position: "absolute", top: 0, left: 0, right: 0,
                    height: 3,
                    background: `linear-gradient(90deg, transparent, ${GOLD}, transparent)`,
                }} />

                {/* Logo + brand */}
                <Box>
                    <Box
                        component="img"
                        src="/images/TheGrandHoTram.png"
                        alt="The Grand Ho Tram"
                        sx={{
                            height: 100,
                            mb: 4,
                            filter: "brightness(0) invert(1)",
                            opacity: 0.95,
                        }}
                    />
                    <Typography
                        variant="h3"
                        sx={{
                            fontWeight: 800,
                            lineHeight: 1.2,
                            mb: 1.5,
                            fontSize: { md: "2rem", lg: "2.4rem" },
                            color: GOLD_LIGHT, display: "block"
                        }}
                    >
                        Digital Document Platform
                        {/* <Box component="span" sx={{ color: "#fff", display: "block" }}>
                            ( HTR Portal )
                        </Box> */}
                    </Typography>
                    <Typography
                        variant="body1"
                        sx={{ color: "rgba(255,255,255,0.65)", maxWidth: 380, lineHeight: 1.7 }}
                    >
                        Safe, secure, and efficient digital document management for The Grand Ho Tram <br />
                        Streamline your operations with real-time tracking, digital signatures, and comprehensive reporting.
                    </Typography>
                </Box>

                {/* Feature list */}
                <Box sx={{ display: "flex", flexDirection: "column", gap: 2.5 }}>
                    <Divider sx={{ borderColor: `${GOLD}44`, mb: 0.5 }} />
                    {features.map((f) => (
                        <Box key={f.label} sx={{ display: "flex", alignItems: "flex-start", gap: 2 }}>
                            <Box
                                sx={{
                                    mt: 0.2,
                                    width: 40, height: 40,
                                    borderRadius: 2,
                                    background: "rgba(255,255,255,0.06)",
                                    border: `1px solid ${GOLD}33`,
                                    display: "flex", alignItems: "center", justifyContent: "center",
                                    flexShrink: 0,
                                }}
                            >
                                {f.icon}
                            </Box>
                            <Box>
                                <Typography sx={{ color: "#fff", fontWeight: 600, fontSize: "0.9rem", lineHeight: 1.3 }}>
                                    {f.label}
                                </Typography>
                                <Typography sx={{ color: "rgba(255,255,255,0.5)", fontSize: "0.8rem", mt: 0.3 }}>
                                    {f.desc}
                                </Typography>
                            </Box>
                        </Box>
                    ))}
                </Box>

                {/* Footer */}
                <Typography sx={{ color: "rgba(255,255,255,0.3)", fontSize: "0.75rem", mt: 4 }}>
                    © {new Date().getFullYear()} The Grand Ho Tram Strip · Authorized Personnel Only
                </Typography>
            </Box>

            {/* ── Right login panel ── */}
            <Box
                sx={{
                    flex: 1,
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    justifyContent: "center",
                    bgcolor: "#f8fafc",
                    px: { xs: 3, sm: 6, md: 7 },
                    py: 6,
                    position: "relative",
                }}
            >
                {/* Mobile logo */}
                <Box
                    component="img"
                    src="/images/TheGrandHoTram.png"
                    alt="The Grand Ho Tram"
                    sx={{
                        display: { xs: "block", md: "none" },
                        height: 44,
                        mb: 4,
                        filter: `invert(27%) sepia(31%) saturate(452%) hue-rotate(147deg) brightness(92%) contrast(96%)`,
                    }}
                />

                <Box sx={{ width: "100%", maxWidth: 420 }}>
                    {/* Header */}
                    <Box sx={{ mb: 4 }}>
                        <Chip
                            label="Secure Portal"
                            size="small"
                            sx={{
                                bgcolor: `${muiTheme.palette.secondary.main}18`,
                                color: muiTheme.palette.secondary.main,
                                fontWeight: 700,
                                fontSize: "0.7rem",
                                letterSpacing: 1,
                                mb: 2,
                                border: `1px solid ${muiTheme.palette.secondary.main}44`,
                            }}
                        />
                        <Typography
                            variant="h4"
                            sx={{ fontWeight: 800, color: muiTheme.palette.primary.dark, lineHeight: 1.2, mb: 0.75 }}
                        >
                            Welcome back
                        </Typography>
                        <Typography variant="body2" sx={{ color: "#64748b" }}>
                            Sign in to your account to continue
                        </Typography>
                    </Box>

                    {/* Form */}
                    <Box component="form" onSubmit={handleSubmit} noValidate>
                        <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
                            <TextField
                                required
                                fullWidth
                                label="Username"
                                value={username}
                                onChange={(e) => {
                                    setUserName(e.target.value);
                                    if (error) setError(null);
                                }}
                                disabled={loading}
                                autoComplete="username"
                                autoFocus
                                error={!!error && !username.trim()}
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <PersonOutlineIcon sx={{ color: muiTheme.palette.primary.main, fontSize: 20 }} />
                                        </InputAdornment>
                                    ),
                                }}
                                sx={{
                                    "& .MuiOutlinedInput-root": {
                                        borderRadius: 2,
                                        bgcolor: "#fff",
                                        transition: "box-shadow 0.2s",
                                        "&:hover fieldset": { borderColor: muiTheme.palette.primary.main },
                                        "&.Mui-focused fieldset": { borderColor: muiTheme.palette.primary.main },
                                        "&.Mui-focused": {
                                            boxShadow: `0 0 0 3px ${muiTheme.palette.primary.main}18`,
                                        },
                                    },
                                    "& .MuiInputLabel-root.Mui-focused": { color: muiTheme.palette.primary.main },
                                }}
                            />

                            <TextField
                                required
                                fullWidth
                                type={showPwd ? "text" : "password"}
                                label="Password"
                                value={password}
                                onChange={(e) => {
                                    setPassword(e.target.value);
                                    if (error) setError(null);
                                }}
                                disabled={loading}
                                autoComplete="current-password"
                                error={!!error && !password.trim()}
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <LockOutlinedIcon sx={{ color: muiTheme.palette.primary.main, fontSize: 20 }} />
                                        </InputAdornment>
                                    ),
                                    endAdornment: (
                                        <InputAdornment position="end">
                                            <IconButton
                                                aria-label="toggle password visibility"
                                                onClick={() => setShowPwd((show) => !show)}
                                                edge="end"
                                                size="small"
                                                disabled={loading}
                                                sx={{ color: "#94a3b8" }}
                                            >
                                                {showPwd ? <VisibilityOff fontSize="small" /> : <Visibility fontSize="small" />}
                                            </IconButton>
                                        </InputAdornment>
                                    ),
                                }}
                                sx={{
                                    "& .MuiOutlinedInput-root": {
                                        borderRadius: 2,
                                        bgcolor: "#fff",
                                        transition: "box-shadow 0.2s",
                                        "&:hover fieldset": { borderColor: muiTheme.palette.primary.main },
                                        "&.Mui-focused fieldset": { borderColor: muiTheme.palette.primary.main },
                                        "&.Mui-focused": {
                                            boxShadow: `0 0 0 3px ${muiTheme.palette.primary.main}18`,
                                        },
                                    },
                                    "& .MuiInputLabel-root.Mui-focused": { color: BRAND_TEAL },
                                }}
                            />
                        </Box>

                        {error && (
                            <Alert
                                severity="error"
                                variant="outlined"
                                sx={{
                                    mt: 2,
                                    borderRadius: 2,
                                    fontSize: "0.85rem",
                                    animation: "fadeSlideIn 0.25s ease-out",
                                    "@keyframes fadeSlideIn": {
                                        from: { opacity: 0, transform: "translateY(-6px)" },
                                        to: { opacity: 1, transform: "translateY(0)" },
                                    },
                                }}
                            >
                                {error}
                            </Alert>
                        )}

                        <Button
                            type="submit"
                            variant="contained"
                            fullWidth
                            size="large"
                            disabled={loading || !username.trim() || !password.trim()}
                            sx={{
                                mt: 3,
                                py: 1.6,
                                borderRadius: 2,
                                fontWeight: 700,
                                fontSize: "0.95rem",
                                letterSpacing: 0.5,
                                background: loading
                                    ? undefined
                                    : `linear-gradient(135deg, ${muiTheme.palette.primary.main} 0%, ${muiTheme.palette.primary.dark} 100%)`,
                                bgcolor: loading ? muiTheme.palette.primary.main : undefined,
                                boxShadow: `0 4px 14px ${muiTheme.palette.primary.main}55`,
                                transition: "all 0.25s ease",
                                "&:hover": {
                                    background: `linear-gradient(135deg, ${muiTheme.palette.primary.dark} 0%, ${muiTheme.palette.primary.main} 100%)`,
                                    boxShadow: `0 6px 20px ${muiTheme.palette.primary.main}70`,
                                    transform: "translateY(-1px)",
                                },
                                "&:active": { transform: "translateY(0)" },
                            }}
                        >
                            {loading ? (
                                <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
                                    <CircularProgress size={18} sx={{ color: "#fff" }} />
                                    <span>Signing in...</span>
                                </Box>
                            ) : (
                                "Sign in"
                            )}
                        </Button>
                    </Box>

                    {/* Divider */}
                    <Divider sx={{ my: 4, borderColor: "#e2e8f0" }}>
                        <Typography sx={{ color: "#94a3b8", fontSize: "0.75rem", px: 1 }}>
                            AUTHORIZED ACCESS ONLY
                        </Typography>
                    </Divider>

                    {/* Footer note */}
                    <Typography
                        variant="caption"
                        sx={{ display: "block", textAlign: "center", color: "#94a3b8", lineHeight: 1.6 }}
                    >
                        This system is for authorized The Grand Ho Tram personnel.<br />
                        All activity is monitored and logged.
                    </Typography>
                </Box>
            </Box>
        </Box>
    );
};

export default Login;
