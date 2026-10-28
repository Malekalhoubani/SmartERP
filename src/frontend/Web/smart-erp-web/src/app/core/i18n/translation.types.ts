export type Language = 'en' | 'ar';
export type TranslationSection = 'labels' | 'menu' | 'alerts';

export interface TranslationResource {
  labels: Record<string, string>;
  menu: Record<string, string>;
  alerts: Record<string, string>;
}

export type TranslationParams = Record<string, string | number>;
