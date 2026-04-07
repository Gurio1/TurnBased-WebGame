export function isCriticalChanceStat(statName: string): boolean {
  return statName === 'Critical chance';
}

export function isCriticalDamageStat(statName: string): boolean {
  return statName === 'Critical damage';
}

export function isCriticalPercentStat(statName: string): boolean {
  return isCriticalChanceStat(statName) || isCriticalDamageStat(statName);
}

export function normalizeCriticalChanceValue(value: number): number {
  if (!Number.isFinite(value)) {
    return 0;
  }

  if (!Number.isInteger(value) && value >= 0 && value <= 1) {
    return Math.round(value * 100);
  }

  return Math.max(0, Math.round(value));
}

export function normalizeCriticalDamageValue(value: number): number {
  if (!Number.isFinite(value)) {
    return 0;
  }

  if (!Number.isInteger(value)) {
    if (value >= 0 && value <= 1) {
      return Math.round(value * 100);
    }

    if (value > 1 && value <= 2) {
      return Math.round((value - 1) * 100);
    }
  }

  return Math.max(0, Math.round(value));
}

export function formatCriticalStatValue(statName: string, value: number): string {
  if (isCriticalChanceStat(statName)) {
    return `${normalizeCriticalChanceValue(value)}%`;
  }

  if (isCriticalDamageStat(statName)) {
    return `${normalizeCriticalDamageValue(value)}%`;
  }

  return `${value}`;
}
