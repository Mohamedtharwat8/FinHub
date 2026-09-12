import { Injectable, signal, computed } from '@angular/core';

export type Language = 'en' | 'ar';

export interface Translations {
  brandName: string;
  home: string;
  accountsBanking: string;
  profileKyc: string;
  settings: string;
  signOut: string;
  searchPlaceholder: string;
  trialBanner: string;
  startSubscription: string;
  welcomeHeading: string;
  welcomeSubtext: string;
  accountCreated: string;
  peopleCreated: string;
  openTicket: string;
  guidanceTitle: string;
  guidanceSubtext: string;
  bankAccountsHeader: string;
  selectAccountSubtext: string;
  openNewAccount: string;
  deposit: string;
  withdraw: string;
  activity: string;
  today: string;
  december13: string;
  loadMore: string;
  engagement: string;
  people: string;
  accounts: string;
  share: string;
  asOf: string;
  noAccounts: string;
}

const EN_DICTIONARY: Translations = {
  brandName: 'FinHub',
  home: 'Home',
  accountsBanking: 'Accounts & Banking',
  profileKyc: 'Profile & KYC',
  settings: 'Settings',
  signOut: 'Sign Out',
  searchPlaceholder: 'Search accounts, transactions...',
  trialBanner: 'FinHub + PayCore workspace',
  startSubscription: 'Start subscription',
  welcomeHeading: 'Hello',
  welcomeSubtext: "Here's what's happening in your FinHub + PayCore workspace today",
  accountCreated: 'Account Created',
  peopleCreated: 'People Created',
  openTicket: 'Open Ticket',
  guidanceTitle: 'FinHub + PayCore guidance',
  guidanceSubtext: 'FinHub banking details and PayCore payment, ledger, fraud and reconciliation flows.',
  bankAccountsHeader: 'Your FinHub Accounts',
  selectAccountSubtext: 'Select an account to view real-time PayCore ledger entries',
  openNewAccount: '+ Open New Account',
  deposit: '+ Deposit',
  withdraw: '- Withdraw',
  activity: 'PayCore Activity',
  today: '🗓 Today',
  december13: '🗓 Today',
  loadMore: 'Load more',
  engagement: 'Engagement',
  people: 'People',
  accounts: 'Accounts',
  share: 'Share',
  asOf: 'as of',
  noAccounts: 'No FinHub accounts found. Click "+ Open New Account" to create a new account.'
};

const AR_DICTIONARY: Translations = {
  brandName: 'فين هاب',
  home: 'الرئيسية',
  accountsBanking: 'الحسابات والخدمات المصرفية',
  profileKyc: 'الملف الشخصي والتحقق',
  settings: 'الإعدادات',
  signOut: 'تسجيل الخروج',
  searchPlaceholder: 'بحث في الحسابات والمعاملات...',
  trialBanner: 'مساحة FinHub + PayCore',
  startSubscription: 'ابدأ الاشتراك',
  welcomeHeading: 'مرحباً',
  welcomeSubtext: 'إليك أبرز نشاطات مساحة FinHub + PayCore اليوم',
  accountCreated: 'الحسابات المنشأة',
  peopleCreated: 'الأشخاص النشطون',
  openTicket: 'تذكرة جديدة',
  guidanceTitle: 'إرشادات FinHub + PayCore',
  guidanceSubtext: 'خدمات FinHub المصرفية ودفتر PayCore للمدفوعات والاحتيال والمطابقة.',
  bankAccountsHeader: 'حسابات FinHub',
  selectAccountSubtext: 'اختر حساباً لعرض دفتر PayCore اللحظي',
  openNewAccount: '+ فتح حساب جديد',
  deposit: '+ إيداع',
  withdraw: '- سحب',
  activity: 'نشاط PayCore',
  today: '🗓 اليوم',
  december13: '🗓 اليوم',
  loadMore: 'عرض المزيد',
  engagement: 'التفاعل',
  people: 'الأفراد',
  accounts: 'الحسابات',
  share: 'مشاركة',
  asOf: 'بتاريخ',
  noAccounts: 'لا توجد حسابات FinHub حتى الآن. انقر على "+ فتح حساب جديد" لإنشاء حساب جديد.'
};

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  public currentLang = signal<Language>('en');

  public dir = computed(() => (this.currentLang() === 'ar' ? 'rtl' : 'ltr'));

  public t = computed<Translations>(() =>
    this.currentLang() === 'ar' ? AR_DICTIONARY : EN_DICTIONARY
  );

  public toggleLanguage(): void {
    const nextLang: Language = this.currentLang() === 'en' ? 'ar' : 'en';
    this.setLanguage(nextLang);
  }

  public setLanguage(lang: Language): void {
    this.currentLang.set(lang);
    if (typeof document !== 'undefined') {
      document.documentElement.dir = this.dir();
      document.documentElement.lang = lang;
    }
  }
}
