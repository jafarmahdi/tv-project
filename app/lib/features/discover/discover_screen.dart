import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/widgets/async_value_view.dart';
import '../../core/widgets/poster_card.dart';
import 'discover_providers.dart';

class DiscoverScreen extends ConsumerStatefulWidget {
  const DiscoverScreen({super.key});

  @override
  ConsumerState<DiscoverScreen> createState() => _DiscoverScreenState();
}

class _DiscoverScreenState extends ConsumerState<DiscoverScreen> with SingleTickerProviderStateMixin {
  late final TabController _tabController;
  final _searchController = TextEditingController();
  Timer? _debounce;

  @override
  void initState() {
    super.initState();
    _tabController = TabController(length: 2, vsync: this);
  }

  @override
  void dispose() {
    _tabController.dispose();
    _searchController.dispose();
    _debounce?.cancel();
    super.dispose();
  }

  void _onSearchChanged(String value) {
    _debounce?.cancel();
    _debounce = Timer(const Duration(milliseconds: 400), () {
      ref.read(discoverQueryProvider.notifier).set(value);
    });
  }

  @override
  Widget build(BuildContext context) {
    final strings = AppStrings.of(context);
    final query = ref.watch(discoverQueryProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(strings.navDiscover, style: const TextStyle(fontWeight: FontWeight.w800)),
        bottom: TabBar(
          controller: _tabController,
          tabs: [Tab(text: strings.moviesTab), Tab(text: strings.seriesTab)],
        ),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
            child: TextField(
              controller: _searchController,
              onChanged: _onSearchChanged,
              decoration: InputDecoration(
                hintText: strings.discoverSearchHint,
                prefixIcon: const Icon(Icons.search),
                suffixIcon: query.isNotEmpty
                    ? IconButton(
                        icon: const Icon(Icons.clear),
                        onPressed: () {
                          _searchController.clear();
                          ref.read(discoverQueryProvider.notifier).set('');
                        },
                      )
                    : null,
              ),
            ),
          ),
          Expanded(
            child: TabBarView(
              controller: _tabController,
              children: [_MoviesGrid(), _SeriesGrid()],
            ),
          ),
        ],
      ),
    );
  }
}

class _MoviesGrid extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final movies = ref.watch(discoverMoviesProvider);
    return AsyncValueView(
      value: movies,
      loadingHeight: 400,
      onRetry: () => ref.invalidate(discoverMoviesProvider),
      data: (result) => GridView.builder(
        padding: const EdgeInsets.all(16),
        gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: 150, mainAxisExtent: 250, crossAxisSpacing: 12, mainAxisSpacing: 16),
        itemCount: result.items.length,
        itemBuilder: (context, index) {
          final movie = result.items[index];
          return PosterCard(
            width: double.infinity,
            posterUrl: TmdbImages.poster(movie.posterPath),
            title: movie.title,
            rating: movie.voteAverage,
            onTap: () => context.push('/movie/${movie.tmdbId}'),
          );
        },
      ),
    );
  }
}

class _SeriesGrid extends ConsumerWidget {
  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final series = ref.watch(discoverSeriesProvider);
    return AsyncValueView(
      value: series,
      loadingHeight: 400,
      onRetry: () => ref.invalidate(discoverSeriesProvider),
      data: (result) => GridView.builder(
        padding: const EdgeInsets.all(16),
        gridDelegate: const SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: 150, mainAxisExtent: 250, crossAxisSpacing: 12, mainAxisSpacing: 16),
        itemCount: result.items.length,
        itemBuilder: (context, index) {
          final show = result.items[index];
          return PosterCard(
            width: double.infinity,
            posterUrl: TmdbImages.poster(show.posterPath),
            title: show.title,
            rating: show.voteAverage,
            onTap: () => context.push('/series/${show.tmdbId}'),
          );
        },
      ),
    );
  }
}
