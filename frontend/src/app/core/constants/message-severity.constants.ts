export const MessageSeverity = {
  SUCCESS: 'success',
  ERROR: 'error',
  WARN: 'warn',
  INFO: 'info',
} as const;

export type MessageSeverityType = typeof MessageSeverity[keyof typeof MessageSeverity];
