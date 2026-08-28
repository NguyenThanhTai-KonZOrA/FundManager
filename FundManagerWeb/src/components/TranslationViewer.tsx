import React from 'react';
import DOMPurify from 'dompurify';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Box, Typography, Tabs, Tab, Divider } from '@mui/material';

interface Props {
    open: boolean;
    onClose: () => void;
    title?: string;
    languageCode?: string;
    contentHtml?: string;
    description?: string | null;
    questionsTranslation?: string | null;
}

export default function TranslationViewer({ open, onClose, title, languageCode, contentHtml, description, questionsTranslation }: Props) {
    const [tab, setTab] = React.useState(0);

    let parsed: any = null;
    let parseError: string | null = null;
    if (questionsTranslation) {
        try {
            parsed = JSON.parse(questionsTranslation);
        } catch (e: any) {
            parseError = e?.message ?? String(e);
        }
    }

    React.useEffect(() => {
        if (!open) setTab(0);
    }, [open]);

    return (
        <Dialog open={open} onClose={onClose} maxWidth="md" fullWidth>
            <DialogTitle>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                    <Typography fontWeight={700}>{title}</Typography>
                    {languageCode && <Typography variant="caption" color="text.secondary">{` · ${languageCode.toUpperCase()}`}</Typography>}
                </Box>
            </DialogTitle>
            <DialogContent dividers>
                <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 1 }}>
                    <Tab label="Content" />
                    <Tab label="Questions" />
                </Tabs>

                {tab === 0 && (
                    <Box>
                        {description && (
                            <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>{description}</Typography>
                        )}
                        <Box sx={{ bgcolor: '#fff', p: 2, border: '1px solid #eee' }}>
                            <div dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(contentHtml ?? '<i>No content</i>') }} />
                        </Box>
                    </Box>
                )}

                {tab === 1 && (
                    <Box>
                        {questionsTranslation == null ? (
                            <Typography variant="caption" color="text.disabled">No questions translation provided.</Typography>
                        ) : parseError ? (
                            <Typography color="error">Invalid JSON: {parseError}</Typography>
                        ) : (
                            <>
                                <Typography variant="subtitle2" sx={{ mb: 1 }}>Preview</Typography>
                                {Array.isArray(parsed) ? (
                                    <Box>
                                        {parsed.map((q: any, idx: number) => (
                                            <Box key={idx} sx={{ mb: 1 }}>
                                                <Typography fontWeight={600}>
                                                    {q.questionId ? `Q${q.questionId}` : q.id ? `Q${q.id}` : `#${idx + 1}`}: {q.questionText ?? q.question ?? '—'}
                                                </Typography>
                                                {Array.isArray(q.options) && q.options.length > 0 && (
                                                    <Box sx={{ pl: 2 }}>
                                                        {q.options.map((opt: any, i: number) => {
                                                            // Handle both string and object options
                                                            const optionText = typeof opt === 'string' ? opt : (opt?.optionText ?? opt?.text ?? String(opt));
                                                            return (
                                                                <Typography key={i} variant="body2">• {optionText}</Typography>
                                                            );
                                                        })}
                                                    </Box>
                                                )}
                                            </Box>
                                        ))}
                                    </Box>
                                ) : (
                                    <Typography variant="body2" fontFamily="monospace" sx={{ whiteSpace: 'pre-wrap' }}>{JSON.stringify(parsed, null, 2)}</Typography>
                                )}

                                <Divider sx={{ my: 2 }} />
                                <Typography variant="subtitle2">Raw JSON</Typography>
                                <Box component="pre" sx={{ fontFamily: 'monospace', fontSize: 12, bgcolor: '#f9f9f9', p: 1, borderRadius: 1, overflow: 'auto', maxHeight: 300 }}>
                                    {JSON.stringify(parsed, null, 2)}
                                </Box>
                            </>
                        )}
                    </Box>
                )}
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose}>Close</Button>
            </DialogActions>
        </Dialog>
    );
}
