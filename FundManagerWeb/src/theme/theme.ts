import { createTheme } from '@mui/material/styles';
import type { ThemeOptions } from '@mui/material/styles';

const baseTheme: ThemeOptions = {

};

export const lightTheme = createTheme({
  ...baseTheme,
  "palette": {
    "mode": "light",
    "primary": {
      "main": "#0069ff"
    },
    "secondary": {
      "main": "#463aa2"
    },
    "error": {
      "main": "#e58b8b"
    },
    "warning": {
      "main": "#efd7bc"
    },
    "info": {
      "main": "#94e7fb"
    },
    "success": {
      "main": "#81cfd1"
    },
    "background": {
      "default": "#f2f7fe",
      "paper": "#ffffff"
    },
    "divider": "rgba(227, 233, 244, 0.12)",
    "text": {
      "primary": "rgba(57, 78, 106, 0.87)",
      "secondary": "rgba(57, 78, 106, 0.75)"
    },
    "action": {
      "active": "rgba(57, 78, 106, 0.54)",
      "hover": "rgba(57, 78, 106, 0.04)",
      "selected": "rgba(57, 78, 106, 0.08)",
      "disabled": "rgba(57, 78, 106, 0.26)",
      "disabledBackground": "rgba(57, 78, 106, 0.12)",
      "focus": "rgba(57, 78, 106, 0.12)"
    }
  },
  "typography": {
    "fontFamily": "\"Just Me Again Down Here\", \"Helvetica Neue\", Arial, sans-serif",
    "fontSize": 14,
    "h1": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 300,
      "fontSize": "6rem",
      "lineHeight": 1.167,
      "letterSpacing": "-0.01562em"
    },
    "h2": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 300,
      "fontSize": "3.75rem",
      "lineHeight": 1.2,
      "letterSpacing": "-0.00833em"
    },
    "h3": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 400,
      "fontSize": "3rem",
      "lineHeight": 1.167,
      "letterSpacing": "0em"
    },
    "h4": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 400,
      "fontSize": "2.125rem",
      "lineHeight": 1.235,
      "letterSpacing": "0.00735em"
    },
    "h5": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 400,
      "fontSize": "1.5rem",
      "lineHeight": 1.334,
      "letterSpacing": "0em"
    },
    "h6": {
      "fontFamily": "\"Roboto\", \"Helvetica\", \"Arial\", sans-serif",
      "fontWeight": 500,
      "fontSize": "1.25rem",
      "lineHeight": 1.6,
      "letterSpacing": "0.0075em"
    }
  },
  "shape": {
    "borderRadius": 8
  },
  "components": {
    "MuiCard": {
      "styleOverrides": {
        "root": {
          "borderRadius": 16
        }
      }
    },
    "MuiDialog": {
      "styleOverrides": {
        "paper": {
          "borderRadius": 16
        }
      }
    },
    "MuiAlert": {
      "styleOverrides": {
        "root": {
          "borderRadius": 16
        }
      }
    },
    "MuiButton": {
      "styleOverrides": {
        "root": {
          "borderRadius": 8,
          "minHeight": 43,
          "paddingInline": 17
        },
        "outlined": {
          "borderWidth": 1
        }
      }
    },
    "MuiOutlinedInput": {
      "styleOverrides": {
        "root": {
          "borderRadius": 8
        },
        "input": {
          "paddingTop": 10,
          "paddingBottom": 10
        },
        "notchedOutline": {
          "borderWidth": 1
        }
      }
    },
    "MuiTabs": {
      "styleOverrides": {
        "root": {
          "minHeight": 43
        }
      }
    },
    "MuiTab": {
      "styleOverrides": {
        "root": {
          "minHeight": 43
        }
      }
    },
    "MuiChip": {
      "styleOverrides": {
        "root": {
          "borderRadius": 16,
          "height": 24,
          "fontSize": 12
        }
      }
    },
    "MuiBadge": {
      "styleOverrides": {
        "badge": {
          "borderRadius": 16,
          "minWidth": 24,
          "height": 24,
          "fontSize": 11
        }
      }
    },
    "MuiSwitch": {
      "styleOverrides": {
        "root": {
          "transform": "scale(1.00)",
          "transformOrigin": "left center"
        }
      }
    },
    "MuiCheckbox": {
      "styleOverrides": {
        "root": {
          "& .MuiSvgIcon-root": {
            "fontSize": 24
          }
        }
      }
    },
    "MuiRadio": {
      "styleOverrides": {
        "root": {
          "& .MuiSvgIcon-root": {
            "fontSize": 24
          }
        }
      }
    }
  }
});

export const darkTheme = createTheme({
  ...baseTheme,
  palette: {
    mode: 'dark',
    primary: {
      main: '#3d5a5f',
      dark: '#274549',
      light: '#5a7a80',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#4fb3d4',
      dark: '#0d93c9',
      light: '#7cc4df',
      contrastText: '#000000',
    },
    background: {
      default: '#0f172a',
      paper: '#1e293b',
    },
    text: {
      primary: '#f1f5f9',
      secondary: '#94a3b8',
    },
  },
});

// Helper to build a theme with an overridden primary color
export const buildTheme = (mode: 'light' | 'dark', primaryMain?: string) => {
  const base = mode === 'light' ? lightTheme : darkTheme;
  return createTheme(base, {
    palette: {
      primary: {
        main: primaryMain || (base.palette as any).primary.main,
        dark: (base.palette as any).primary.dark,
        light: (base.palette as any).primary.light,
        contrastText: (base.palette as any).primary.contrastText,
      },
    },
  });
};

// Keep backward compatibility
export const theme = lightTheme;
