import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/widgets/async_value_view.dart';
import '../../core/widgets/poster_card.dart';
import '../../core/widgets/section_header.dart';
import 'home_providers.dart';

class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final trendingMovies = ref.watch(trendingMoviesProvider);
    final trendingSeries = ref.watch(trendingSeriesProvider);
    final unreadCount = ref.watch(unreadNotificationCountProvider);

    return Scaffold(
      body: RefreshIndicator(
        onRefresh: () async {
          ref.invalidate(trendingMoviesProvider);
          ref.invalidate(trendingSeriesProvider);
        },
        child: CustomScrollView(
          slivers: [
            SliverAppBar(
              floating: true,
              title: Text(strings.appName, style: const TextStyle(fontWeight: FontWeight.w800)),
              actions: [
                IconButton(
                  icon: unreadCount.maybeWhen(
                    data: (count) => count > 0
                        ? Badge(label: Text('$count'), child: const Icon(Icons.notifications_outlined))
                        : const Icon(Icons.notifications_outlined),
                    orElse: () => const Icon(Icons.notifications_outlined),
                  ),
                  onPressed: () => context.push('/notifications'),
                ),
                IconButton(
                  icon: const Icon(Icons.auto_awesome_outlined),
                  onPressed: () => context.push('/ai-assistant'),
                ),
                const SizedBox(width: 8),
              ],
            ),
            SliverToBoxAdapter(child: SectionHeader(title: strings.trendingMovies)),
            SliverToBoxAdapter(
              child: SizedBox(
                height: 260,
                child: AsyncValueView(
                  value: trendingMovies,
                  onRetry: () => ref.invalidate(trendingMoviesProvider),
                  data: (result) => ListView.separated(
                    scrollDirection: Axis.horizontal,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: result.items.length,
                    separatorBuilder: (_, __) => const SizedBox(width: 12),
                    itemBuilder: (context, index) {
                      final movie = result.items[index];
                      return PosterCard(
                        posterUrl: TmdbImages.poster(movie.posterPath),
                        title: movie.title,
                        subtitle: movie.genres.isNotEmpty ? movie.genres.first : null,
                        rating: movie.voteAverage,
                        onTap: () => context.push('/movie/${movie.tmdbId}'),
                      );
                    },
                  ),
                ),
              ),
            ),
            SliverToBoxAdapter(child: SectionHeader(title: strings.trendingSeries)),
            SliverToBoxAdapter(
              child: SizedBox(
                height: 260,
                child: AsyncValueView(
                  value: trendingSeries,
                  onRetry: () => ref.invalidate(trendingSeriesProvider),
                  data: (result) => ListView.separated(
                    scrollDirection: Axis.horizontal,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: result.items.length,
                    separatorBuilder: (_, __) => const SizedBox(width: 12),
                    itemBuilder: (context, index) {
                      final series = result.items[index];
                      return PosterCard(
                        posterUrl: TmdbImages.poster(series.posterPath),
                        title: series.title,
                        subtitle: series.genres.isNotEmpty ? series.genres.first : null,
                        rating: series.voteAverage,
                        onTap: () => context.push('/series/${series.tmdbId}'),
                      );
                    },
                  ),
                ),
              ),
            ),
            const SliverToBoxAdapter(child: SizedBox(height: 24)),
          ],
        ),
      ),
    );
  }
}
