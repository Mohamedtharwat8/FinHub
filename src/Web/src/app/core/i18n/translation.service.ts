import { Injectable, signal, computed } from '@angular/core';

export type Language = 'en' | 'ar';

export interface Translations {
  brandName: string;
  home: string;
  accountsBanking: string;
  profileKyc: string;
  transfersEmail: string;
  helpDesk: string;
  billing: string;
  authentication: string;
  reports: string;
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
  integratedCircleTitle: string;
  integratedCircleDesc: string;
  integratedDiscordTitle: string;
  integratedDiscordDesc: string;
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
  emails: string;
  tickets: string;
  share: string;
  asOf: string;
  noAccounts: string;
}

const EN_DICTIONARY: Translations = {
  brandName: 'FinHub',
  home: 'Home',
  accountsBanking: 'Accounts & Banking',
  profileKyc: 'Profile & KYC',
  transfersEmail: 'Transfers & Email',
  helpDesk: 'Help Desk',
  billing: 'Billing',
  authentication: 'Authentication',
  reports: 'Reports',
  settings: 'Settings',
  signOut: 'Sign Out',
  searchPlaceholder: 'Search accounts, transactions...',
  trialBanner: 'During your trial you have 25 free contacts. You can start your subscription now if you would like to import or email the 1000 contacts that come with your plan.',
  startSubscription: 'Start your subscription ›',
  welcomeHeading: 'Hey there',
  welcomeSubtext: "Here's what's happening in your FinHub account today",
  accountCreated: 'Account Created',
  peopleCreated: 'People Created',
  openTicket: 'Open Ticket',
  guidanceTitle: 'A little guidance',
  guidanceSubtext: "Since you told us you're building a community...",
  integratedCircleTitle: 'Integrated FinHub with SAMA',
  integratedCircleDesc: 'Monetize and manage your Saudi Open Banking community with SSO integration',
  integratedDiscordTitle: 'Integrated FinHub with Discord',
  integratedDiscordDesc: 'Automate roles, manage members and instant notifications via Discord bot',
  bankAccountsHeader: 'Your Bank Accounts (SAMA IBAN)',
  selectAccountSubtext: 'Select an account to view real-time ledger entries',
  openNewAccount: '+ Open New Account',
  deposit: '+ Deposit',
  withdraw: '- Withdraw',
  activity: 'Activity',
  today: '🗓 Today',
  december13: '🗓 December 13',
  loadMore: 'Load more',
  engagement: 'Engagement',
  people: 'People',
  accounts: 'Accounts',
  emails: 'Emails',
  tickets: 'Tickets',
  share: 'Share',
  asOf: 'as of',
  noAccounts: 'No bank accounts found. Click "+ Open New Account" to generate your Saudi IBAN.'
};

const AR_DICTIONARY: Translations = {
  brandName: 'فين هاب',
  home: 'الرئيسية',
  accountsBanking: 'الحسابات والخدمات المصرفية',
  profileKyc: 'الملف الشخصي والتحقق',
  transfersEmail: 'التحويلات والبريد',
  helpDesk: 'مركز المساعدة',
  billing: 'الفواتير والاشتراكات',
  authentication: 'المصادقة والأمان',
  reports: 'التقارير والتحليلات',
  settings: 'الإعدادات',
  signOut: 'تسجيل الخروج',
  searchPlaceholder: 'بحث في الحسابات والمعاملات...',
  trialBanner: 'خلال الفترة التجريبية لديك 25 جهة اتصال مجانية. يمكنك بدء اشتراكك الآن لاستيراد 1000 جهة اتصال في خطتك.',
  startSubscription: 'بدء الاشتراك الآن ›',
  welcomeHeading: 'أهلاً بك',
  welcomeSubtext: 'إليك نظرة عامة على نشاط حسابك في فين هاب اليوم',
  accountCreated: 'الحسابات المنشأة',
  peopleCreated: 'المستخدمين النشطين',
  openTicket: 'التذاكر المفتوحة',
  guidanceTitle: 'إرشادات سريعة',
  guidanceSubtext: 'بناءً على اختيارك لتطوير المنظومة المالية...',
  integratedCircleTitle: 'الربط مع البنك المركزي (ساما)',
  integratedCircleDesc: 'إدارة وتكامل الخدمات المصرفية المفتوحة وفق المعايير السعودية',
  integratedDiscordTitle: 'التكامل مع الإشعارات الفورية',
  integratedDiscordDesc: 'إرسال تنبيهات سحب وإيداع فورية عبر المساعد الذكي',
  bankAccountsHeader: 'حساباتك المصرفية (آيبان سعودي)',
  selectAccountSubtext: 'اختر حساباً لعرض السجل المالي والعمليات المباشرة',
  openNewAccount: '+ فتح حساب جديد',
  deposit: '+ إيداع',
  withdraw: '- سحب',
  activity: 'الأنشطة الأخيرة',
  today: '🗓 اليوم',
  december13: '🗓 13 ديسمبر',
  loadMore: 'عرض المزيد',
  engagement: 'التفاعل',
  people: 'الأفراد',
  accounts: 'الحسابات',
  emails: 'البريد',
  tickets: 'التذاكر',
  share: 'مشاركة',
  asOf: 'بتاريخ',
  noAccounts: 'لا توجد حسابات مصرفية. انقر على "+ فتح حساب جديد" لإنشاء آيبان سعودي.'
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
