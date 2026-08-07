import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../core/localization/app_strings.dart';
import '../../core/models/admin_models.dart';
import '../../core/network/api_exception.dart';
import '../../core/providers/core_providers.dart';
import '../../core/theme/app_theme.dart';
import '../auth/auth_provider.dart';

class AdminToolsScreen extends ConsumerStatefulWidget {
  const AdminToolsScreen({super.key});

  @override
  ConsumerState<AdminToolsScreen> createState() => _AdminToolsScreenState();
}

class _AdminToolsScreenState extends ConsumerState<AdminToolsScreen> {
  final _movieTmdbIdController = TextEditingController();
  final _seriesTmdbIdController = TextEditingController();
  final _episodeSeriesTmdbIdController = TextEditingController();
  final _episodeSeasonController = TextEditingController(text: '1');
  final _episodeNumberController = TextEditingController(text: '1');
  final _moviesYearController = TextEditingController(
    text: '${DateTime.now().year}',
  );
  final _moviesPagesController = TextEditingController(text: '2');
  final _seriesYearController = TextEditingController(
    text: '${DateTime.now().year}',
  );
  final _seriesPagesController = TextEditingController(text: '2');

  bool _isBusy = false;
  ImportedCatalogItem? _lastImportedItem;
  ImportRunResult? _lastImportRun;
  String? _lastAction;

  @override
  void dispose() {
    _movieTmdbIdController.dispose();
    _seriesTmdbIdController.dispose();
    _episodeSeriesTmdbIdController.dispose();
    _episodeSeasonController.dispose();
    _episodeNumberController.dispose();
    _moviesYearController.dispose();
    _moviesPagesController.dispose();
    _seriesYearController.dispose();
    _seriesPagesController.dispose();
    super.dispose();
  }

  Future<void> _runAction(
    String action,
    Future<void> Function() callback,
  ) async {
    setState(() => _isBusy = true);
    try {
      await callback();
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(AppStrings.of(context).importCompleted)),
        );
      }
    } catch (error) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              error is ApiException ? error.message : 'Something went wrong.',
            ),
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isBusy = false;
          _lastAction = action;
        });
      }
    }
  }

  int? _parseInt(TextEditingController controller) {
    final value = controller.text.trim();
    if (value.isEmpty) return null;
    return int.tryParse(value);
  }

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final profile = ref.watch(authProvider).profile;
    final isAdmin = profile?.isAdmin ?? false;
    final scheme = Theme.of(context).colorScheme;

    return Scaffold(
      appBar: AppBar(title: Text(strings.adminTools)),
      body: !isAdmin
          ? Center(
              child: Padding(
                padding: const EdgeInsets.all(24),
                child: _InfoCard(
                  icon: Icons.admin_panel_settings_outlined,
                  title: strings.adminOnly,
                  subtitle: strings.adminOnlyHint,
                ),
              ),
            )
          : ListView(
              padding: const EdgeInsets.fromLTRB(16, 12, 16, 24),
              children: [
                Container(
                  padding: const EdgeInsets.all(20),
                  decoration: BoxDecoration(
                    gradient: LinearGradient(
                      begin: Alignment.topLeft,
                      end: Alignment.bottomRight,
                      colors: [
                        scheme.primaryContainer,
                        scheme.secondaryContainer,
                      ],
                    ),
                    borderRadius: BorderRadius.circular(AppTheme.radiusLg),
                  ),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        strings.adminTools,
                        style: Theme.of(context).textTheme.headlineSmall
                            ?.copyWith(fontWeight: FontWeight.w900),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        strings.adminSubtitle,
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: scheme.onSecondaryContainer,
                          height: 1.4,
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 20),
                Text(
                  strings.directImport,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 10),
                _ImportCard(
                  title: strings.importMovie,
                  child: Column(
                    children: [
                      TextField(
                        controller: _movieTmdbIdController,
                        keyboardType: TextInputType.number,
                        decoration: InputDecoration(
                          labelText: strings.movieTmdbId,
                        ),
                      ),
                      const SizedBox(height: 12),
                      Align(
                        alignment: AlignmentDirectional.centerEnd,
                        child: FilledButton.icon(
                          onPressed: _isBusy
                              ? null
                              : () {
                                  final tmdbId = _parseInt(
                                    _movieTmdbIdController,
                                  );
                                  if (tmdbId == null) return;
                                  _runAction(strings.importMovie, () async {
                                    final result = await ref
                                        .read(adminApiProvider)
                                        .importMovie(tmdbId);
                                    setState(() {
                                      _lastImportedItem = result;
                                      _lastImportRun = null;
                                    });
                                  });
                                },
                          icon: const Icon(Icons.movie_creation_outlined),
                          label: Text(strings.importMovie),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                _ImportCard(
                  title: strings.importSeries,
                  child: Column(
                    children: [
                      TextField(
                        controller: _seriesTmdbIdController,
                        keyboardType: TextInputType.number,
                        decoration: InputDecoration(
                          labelText: strings.seriesTmdbId,
                        ),
                      ),
                      const SizedBox(height: 12),
                      Align(
                        alignment: AlignmentDirectional.centerEnd,
                        child: FilledButton.icon(
                          onPressed: _isBusy
                              ? null
                              : () {
                                  final tmdbId = _parseInt(
                                    _seriesTmdbIdController,
                                  );
                                  if (tmdbId == null) return;
                                  _runAction(strings.importSeries, () async {
                                    final result = await ref
                                        .read(adminApiProvider)
                                        .importSeries(tmdbId);
                                    setState(() {
                                      _lastImportedItem = result;
                                      _lastImportRun = null;
                                    });
                                  });
                                },
                          icon: const Icon(Icons.tv_outlined),
                          label: Text(strings.importSeries),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                _ImportCard(
                  title: strings.importEpisode,
                  child: Column(
                    children: [
                      TextField(
                        controller: _episodeSeriesTmdbIdController,
                        keyboardType: TextInputType.number,
                        decoration: InputDecoration(
                          labelText: strings.seriesTmdbId,
                        ),
                      ),
                      const SizedBox(height: 12),
                      Row(
                        children: [
                          Expanded(
                            child: TextField(
                              controller: _episodeSeasonController,
                              keyboardType: TextInputType.number,
                              decoration: InputDecoration(
                                labelText: strings.seasonNumber,
                              ),
                            ),
                          ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: TextField(
                              controller: _episodeNumberController,
                              keyboardType: TextInputType.number,
                              decoration: InputDecoration(
                                labelText: strings.episodeNumber,
                              ),
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),
                      Align(
                        alignment: AlignmentDirectional.centerEnd,
                        child: FilledButton.icon(
                          onPressed: _isBusy
                              ? null
                              : () {
                                  final seriesTmdbId = _parseInt(
                                    _episodeSeriesTmdbIdController,
                                  );
                                  final seasonNumber = _parseInt(
                                    _episodeSeasonController,
                                  );
                                  final episodeNumber = _parseInt(
                                    _episodeNumberController,
                                  );
                                  if (seriesTmdbId == null ||
                                      seasonNumber == null ||
                                      episodeNumber == null) {
                                    return;
                                  }
                                  _runAction(strings.importEpisode, () async {
                                    final result = await ref
                                        .read(adminApiProvider)
                                        .importEpisode(
                                          seriesTmdbId: seriesTmdbId,
                                          seasonNumber: seasonNumber,
                                          episodeNumber: episodeNumber,
                                        );
                                    setState(() {
                                      _lastImportedItem = result;
                                      _lastImportRun = null;
                                    });
                                  });
                                },
                          icon: const Icon(
                            Icons.playlist_add_check_circle_outlined,
                          ),
                          label: Text(strings.importEpisode),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 20),
                Text(
                  strings.bulkImport,
                  style: Theme.of(context).textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w800,
                  ),
                ),
                const SizedBox(height: 10),
                _ImportCard(
                  title: strings.importMoviesByYear,
                  child: _BulkImportForm(
                    yearController: _moviesYearController,
                    pagesController: _moviesPagesController,
                    submitLabel: strings.runImport,
                    busy: _isBusy,
                    onSubmit: () {
                      final year = _parseInt(_moviesYearController);
                      final pages = _parseInt(_moviesPagesController);
                      if (year == null || pages == null) return;
                      _runAction(strings.importMoviesByYear, () async {
                        final result = await ref
                            .read(adminApiProvider)
                            .importMoviesByYear(year: year, pages: pages);
                        setState(() {
                          _lastImportRun = result;
                          _lastImportedItem = null;
                        });
                      });
                    },
                  ),
                ),
                const SizedBox(height: 12),
                _ImportCard(
                  title: strings.importSeriesByYear,
                  child: _BulkImportForm(
                    yearController: _seriesYearController,
                    pagesController: _seriesPagesController,
                    submitLabel: strings.runImport,
                    busy: _isBusy,
                    onSubmit: () {
                      final year = _parseInt(_seriesYearController);
                      final pages = _parseInt(_seriesPagesController);
                      if (year == null || pages == null) return;
                      _runAction(strings.importSeriesByYear, () async {
                        final result = await ref
                            .read(adminApiProvider)
                            .importSeriesByYear(year: year, pages: pages);
                        setState(() {
                          _lastImportRun = result;
                          _lastImportedItem = null;
                        });
                      });
                    },
                  ),
                ),
                const SizedBox(height: 20),
                if (_lastImportedItem != null || _lastImportRun != null)
                  _ImportResultCard(
                    action: _lastAction ?? strings.lastRun,
                    importedItem: _lastImportedItem,
                    importRun: _lastImportRun,
                  ),
              ],
            ),
    );
  }
}

class _ImportCard extends StatelessWidget {
  final String title;
  final Widget child;

  const _ImportCard({required this.title, required this.child});

  @override
  Widget build(BuildContext context) {
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              title,
              style: Theme.of(
                context,
              ).textTheme.titleSmall?.copyWith(fontWeight: FontWeight.w800),
            ),
            const SizedBox(height: 12),
            child,
          ],
        ),
      ),
    );
  }
}

class _BulkImportForm extends StatelessWidget {
  final TextEditingController yearController;
  final TextEditingController pagesController;
  final String submitLabel;
  final bool busy;
  final VoidCallback onSubmit;

  const _BulkImportForm({
    required this.yearController,
    required this.pagesController,
    required this.submitLabel,
    required this.busy,
    required this.onSubmit,
  });

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);

    return Column(
      children: [
        Row(
          children: [
            Expanded(
              child: TextField(
                controller: yearController,
                keyboardType: TextInputType.number,
                decoration: InputDecoration(labelText: strings.year),
              ),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: TextField(
                controller: pagesController,
                keyboardType: TextInputType.number,
                decoration: InputDecoration(labelText: strings.pages),
              ),
            ),
          ],
        ),
        const SizedBox(height: 12),
        Align(
          alignment: AlignmentDirectional.centerEnd,
          child: FilledButton.icon(
            onPressed: busy ? null : onSubmit,
            icon: const Icon(Icons.cloud_download_outlined),
            label: Text(submitLabel),
          ),
        ),
      ],
    );
  }
}

class _ImportResultCard extends StatelessWidget {
  final String action;
  final ImportedCatalogItem? importedItem;
  final ImportRunResult? importRun;

  const _ImportResultCard({
    required this.action,
    required this.importedItem,
    required this.importRun,
  });

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final scheme = Theme.of(context).colorScheme;

    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              strings.lastRun,
              style: Theme.of(
                context,
              ).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w800),
            ),
            const SizedBox(height: 8),
            Text(
              action,
              style: Theme.of(
                context,
              ).textTheme.bodyMedium?.copyWith(color: scheme.onSurfaceVariant),
            ),
            if (importedItem != null) ...[
              const SizedBox(height: 14),
              _InfoCard(
                icon: Icons.check_circle_outline,
                title: importedItem!.title,
                subtitle:
                    '${importedItem!.entityType} • ${importedItem!.reference}',
              ),
            ],
            if (importRun != null) ...[
              const SizedBox(height: 14),
              _InfoCard(
                icon: Icons.batch_prediction_outlined,
                title:
                    '${importRun!.itemsImported}/${importRun!.itemsDiscovered} imported',
                subtitle:
                    '${strings.year}: ${importRun!.year} • ${strings.pages}: ${importRun!.pagesRequested}',
              ),
              if (importRun!.errors.isNotEmpty) ...[
                const SizedBox(height: 10),
                ...importRun!.errors
                    .take(5)
                    .map(
                      (error) => Padding(
                        padding: const EdgeInsets.only(bottom: 6),
                        child: Text(
                          '• $error',
                          style: Theme.of(
                            context,
                          ).textTheme.bodySmall?.copyWith(color: scheme.error),
                        ),
                      ),
                    ),
              ],
            ],
          ],
        ),
      ),
    );
  }
}

class _InfoCard extends StatelessWidget {
  final IconData icon;
  final String title;
  final String subtitle;

  const _InfoCard({
    required this.icon,
    required this.title,
    required this.subtitle,
  });

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(14),
      decoration: BoxDecoration(
        color: scheme.surfaceContainerHigh,
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
      ),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: scheme.primaryContainer,
              borderRadius: BorderRadius.circular(AppTheme.radiusSm),
            ),
            child: Icon(icon, color: scheme.onPrimaryContainer),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: Theme.of(
                    context,
                  ).textTheme.bodyLarge?.copyWith(fontWeight: FontWeight.w800),
                ),
                const SizedBox(height: 4),
                Text(
                  subtitle,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                    color: scheme.onSurfaceVariant,
                    height: 1.4,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
