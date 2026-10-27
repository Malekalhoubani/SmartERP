import { DOCUMENT } from '@angular/common';
import { computed, inject, Injectable, signal } from '@angular/core';
import { ar } from './translations/ar';
import { en } from './translations/en';
import {
  Language,
  TranslationParams,
  TranslationResource,
  TranslationSection,
} from './translation.types';

const resources: Record<Language, TranslationResource> = { en, ar };
const languageStorageKey = 'smart-erp-language';

@Injectable({ providedIn: 'root' })
export class TranslationService {
  private readonly document = inject(DOCUMENT);
  private readonly selectedLanguage = signal<Language>(this.getInitialLanguage());

  readonly language = this.selectedLanguage.asReadonly();
  readonly direction = computed(() => (this.language() === 'ar' ? 'rtl' : 'ltr'));
  readonly isArabic = computed(() => this.language() === 'ar');

  constructor() {
    this.applyDocumentLanguage(this.language());
  }

  setLanguage(language: Language): void {
    this.selectedLanguage.set(language);
    localStorage.setItem(languageStorageKey, language);
    this.applyDocumentLanguage(language);
  }

  toggleLanguage(): void {
    this.setLanguage(this.language() === 'en' ? 'ar' : 'en');
  }

  t(section: TranslationSection, key: string, params: TranslationParams = {}): string {
    const template = resources[this.language()][section][key] ?? resources.en[section][key] ?? key;

    return Object.entries(params).reduce(
      (message, [name, value]) => message.replaceAll(`{{${name}}}`, String(value)),
      template,
    );
  }

  private getInitialLanguage(): Language {
    const savedLanguage = localStorage.getItem(languageStorageKey);
    return savedLanguage === 'ar' || savedLanguage === 'en' ? savedLanguage : 'en';
  }

  private applyDocumentLanguage(language: Language): void {
    this.document.documentElement.lang = language;
    this.document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr';
  }
}
