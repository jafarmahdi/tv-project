import 'package:flutter/widgets.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

/// User-selectable UI language. Defaults to the device locale when it's `ar`,
/// otherwise `en`. Driving `MaterialApp.locale` from this also flips text
/// direction to RTL automatically for Arabic — Flutter resolves
/// `Directionality` from the active locale via `flutter_localizations`.
final localeProvider = NotifierProvider<LocaleNotifier, Locale>(
  LocaleNotifier.new,
);

class LocaleNotifier extends Notifier<Locale> {
  @override
  Locale build() => const Locale('en');

  void setLocale(Locale locale) => state = locale;
}

/// A small hand-maintained translation table for the app's chrome (nav,
/// auth, settings, common actions). Deep content (movie/series data) comes
/// from TMDB in whatever language the backend requested, which today is
/// always English — see docs/ROADMAP.md for wiring a `language` param through.
class AppStrings {
  final Locale locale;
  const AppStrings(this.locale);

  static AppStrings of(BuildContext context) =>
      AppStrings(Localizations.localeOf(context));

  bool get isArabic => locale.languageCode == 'ar';

  String _t(String en, String ar) => isArabic ? ar : en;

  String get appName => 'WatchLog';
  String get navHome => _t('Home', 'الرئيسية');
  String get navDiscover => _t('Discover', 'اكتشف');
  String get navStats => _t('Stats', 'الإحصائيات');
  String get navProfile => _t('Profile', 'حسابي');

  String get loginTitle => _t('Welcome back', 'مرحباً بعودتك');
  String get loginSubtitle => _t(
    'Sign in to keep tracking what you watch.',
    'سجّل الدخول لمتابعة ما تشاهده.',
  );
  String get registerTitle => _t('Create your account', 'أنشئ حسابك');
  String get registerSubtitle => _t(
    'Track shows, get AI picks, see your stats.',
    'تابع المسلسلات واحصل على توصيات الذكاء الاصطناعي وشاهد إحصائياتك.',
  );
  String get email => _t('Email', 'البريد الإلكتروني');
  String get password => _t('Password', 'كلمة المرور');
  String get displayName => _t('Display name', 'الاسم الظاهر');
  String get signIn => _t('Sign in', 'تسجيل الدخول');
  String get createAccount => _t('Create account', 'إنشاء حساب');
  String get noAccountYet => _t("Don't have an account? ", 'ما عندك حساب؟ ');
  String get haveAccount => _t('Already have an account? ', 'عندك حساب؟ ');
  String get signUp => _t('Sign up', 'أنشئ واحد');

  String get discoverSearchHint =>
      _t('Search movies & series', 'ابحث عن أفلام ومسلسلات');
  String get moviesTab => _t('Movies', 'أفلام');
  String get seriesTab => _t('Series', 'مسلسلات');
  String get trendingMovies => _t('Trending Movies', 'أفلام رائجة');
  String get trendingSeries => _t('Trending Series', 'مسلسلات رائجة');
  String get popularThisWeek =>
      _t('Popular This Week', 'الأكثر شعبية هذا الأسبوع');
  String get continueWatching => _t('Continue Watching', 'أكمل المشاهدة');
  String get upcomingEpisodes => _t('Upcoming Episodes', 'حلقات قادمة');

  String get statsTitle => _t('Your Statistics', 'إحصائياتك');
  String get totalEpisodes => _t('Episodes Watched', 'الحلقات المشاهدة');
  String get totalMovies => _t('Movies Watched', 'الأفلام المشاهدة');
  String get totalWatchTime => _t('Watch Time', 'وقت المشاهدة');
  String get favoriteGenres => _t('Favorite Genres', 'الأنواع المفضلة');
  String get achievements => _t('Achievements', 'الإنجازات');
  String get community => _t('Community', 'المجتمع');
  String get yourRating => _t('Your rating', 'تقييمك');
  String get communityRating => _t('Community rating', 'تقييم المستخدمين');
  String get rateThis => _t('Rate this', 'قيّم هذا');
  String get writeComment => _t('Write a comment', 'اكتب تعليقاً');
  String get shareThoughts => _t(
    'Share your thoughts about this title...',
    'اكتب رأيك عن هذا العمل...',
  );
  String get post => _t('Post', 'نشر');
  String get noCommentsYet => _t('No comments yet.', 'لا توجد تعليقات بعد.');
  String get startConversation => _t('Start the conversation.', 'ابدأ النقاش.');
  String get joined => _t('Joined', 'انضم');
  String get followers => _t('Followers', 'المتابعون');
  String get following => _t('Following', 'يتابع');
  String get viewAll => _t('View all', 'عرض الكل');
  String get episodeDiscussion => _t('Episode discussion', 'نقاش الحلقة');
  String get ratingsCount => _t('ratings', 'تقييمات');
  String get commentPosted => _t('Comment posted.', 'تم نشر التعليق.');
  String get ratingSaved => _t('Rating saved.', 'تم حفظ التقييم.');
  String get beFirstToRate =>
      _t('Be the first to rate it.', 'كن أول من يقيّمه.');
  String get adminTools => _t('Admin Tools', 'أدوات الأدمن');
  String get adminSubtitle => _t(
    'Import titles into the local catalog and prepare the app content.',
    'استورد الأعمال إلى الكاتالوك المحلي وجهّز محتوى التطبيق.',
  );
  String get directImport => _t('Direct import', 'استيراد مباشر');
  String get bulkImport => _t('Bulk import', 'استيراد جماعي');
  String get movieTmdbId => _t('Movie TMDB ID', 'معرّف TMDB للفيلم');
  String get seriesTmdbId => _t('Series TMDB ID', 'معرّف TMDB للمسلسل');
  String get seasonNumber => _t('Season number', 'رقم الموسم');
  String get episodeNumber => _t('Episode number', 'رقم الحلقة');
  String get importMovie => _t('Import movie', 'استيراد فيلم');
  String get importSeries => _t('Import series', 'استيراد مسلسل');
  String get importEpisode => _t('Import episode', 'استيراد حلقة');
  String get importMoviesByYear =>
      _t('Import movies by year', 'استيراد أفلام حسب السنة');
  String get importSeriesByYear =>
      _t('Import series by year', 'استيراد مسلسلات حسب السنة');
  String get year => _t('Year', 'السنة');
  String get pages => _t('Pages', 'الصفحات');
  String get runImport => _t('Run import', 'ابدأ الاستيراد');
  String get adminOnly => _t('Admin access only', 'للوصول الإداري فقط');
  String get adminOnlyHint => _t(
    'This screen is available only for accounts with the Admin role.',
    'هذه الشاشة متاحة فقط للحسابات التي تملك صلاحية Admin.',
  );
  String get importCompleted => _t('Import completed.', 'اكتمل الاستيراد.');
  String get lastRun => _t('Last run', 'آخر عملية');
  String get openAdminTools => _t('Open admin tools', 'افتح أدوات الأدمن');

  String get settingsTitle => _t('Settings', 'الإعدادات');
  String get language => _t('Language', 'اللغة');
  String get theme => _t('Theme', 'المظهر');
  String get signOut => _t('Sign out', 'تسجيل الخروج');

  String get notifications => _t('Notifications', 'الإشعارات');
  String get aiAssistant => _t('AI Assistant', 'المساعد الذكي');
  String get aiAssistantHint => _t(
    'Ask for a recommendation, e.g. "I have 90 minutes" or "something like Dark"',
    'اطلب توصية، مثلاً "عندي 90 دقيقة" أو "شي يشبه Dark"',
  );

  String get lists => _t('My Lists', 'قوائمي');
  String get watching => _t('Watching', 'أشاهده الآن');
  String get completed => _t('Completed', 'مكتمل');
  String get planned => _t('Planned', 'مخطط له');
  String get onHold => _t('On Hold', 'متوقف مؤقتاً');
  String get dropped => _t('Dropped', 'متروك');
  String get favorites => _t('Favorites', 'المفضلة');

  String get markWatched => _t('Mark watched', 'وسم كمُشاهد');
  String get markSkipped => _t('Skip', 'تخطي');
  String get addToList => _t('Add to list', 'أضف إلى قائمة');
  String get retry => _t('Retry', 'إعادة المحاولة');
  String get seeAll => _t('See all', 'عرض الكل');
  String get cast => _t('Cast', 'طاقم التمثيل');
  String get seasons => _t('Seasons', 'المواسم');
  String get similar => _t('Similar', 'مشابه');
  String get overview => _t('Overview', 'نظرة عامة');
}
