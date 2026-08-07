import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../core/config/tmdb_images.dart';
import '../../core/localization/app_strings.dart';
import '../../core/theme/app_theme.dart';
import '../../core/widgets/async_value_view.dart';
import '../auth/auth_provider.dart';
import 'profile_providers.dart';

class ProfileScreen extends ConsumerWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final strings = AppStrings.of(context);
    final auth = ref.watch(authProvider);
    final profile = auth.profile;
    final lists = ref.watch(myListsProvider);
    final scheme = Theme.of(context).colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: Text(strings.navProfile, style: const TextStyle(fontWeight: FontWeight.w800)),
        actions: [
          IconButton(icon: const Icon(Icons.settings_outlined), onPressed: () => context.push('/settings')),
        ],
      ),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          Row(
            children: [
              CircleAvatar(
                radius: 34,
                backgroundColor: scheme.primaryContainer,
                backgroundImage: profile?.avatarUrl != null ? NetworkImage(profile!.avatarUrl!) : null,
                child: profile?.avatarUrl == null
                    ? Text(
                        (profile?.displayName.isNotEmpty == true ? profile!.displayName[0] : '?').toUpperCase(),
                        style: TextStyle(fontSize: 24, color: scheme.onPrimaryContainer, fontWeight: FontWeight.w700),
                      )
                    : null,
              ),
              const SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(profile?.displayName ?? '', style: Theme.of(context).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.w800)),
                    Text(profile?.email ?? '', style: Theme.of(context).textTheme.bodySmall?.copyWith(color: scheme.onSurfaceVariant)),
                    if (profile != null) ...[
                      const SizedBox(height: 4),
                      Text('${profile.followerCount} followers · ${profile.followingCount} following',
                          style: Theme.of(context).textTheme.bodySmall),
                    ],
                  ],
                ),
              ),
            ],
          ),
          if (profile?.bio != null && profile!.bio!.isNotEmpty) ...[
            const SizedBox(height: 12),
            Text(profile.bio!, style: Theme.of(context).textTheme.bodyMedium),
          ],
          const SizedBox(height: 24),
          Text(strings.lists, style: Theme.of(context).textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w700)),
          const SizedBox(height: 8),
          AsyncValueView(
            value: lists,
            loadingHeight: 150,
            onRetry: () => ref.invalidate(myListsProvider),
            data: (userLists) => Column(
              children: userLists.map((list) => _ListExpansion(list: list)).toList(),
            ),
          ),
        ],
      ),
    );
  }
}

class _ListExpansion extends ConsumerWidget {
  final dynamic list;
  const _ListExpansion({required this.list});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    return Card(
      margin: const EdgeInsets.only(bottom: 8),
      child: ExpansionTile(
        title: Text(list.name as String),
        subtitle: Text('${list.itemCount} items'),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(AppTheme.radiusMd)),
        children: [
          if ((list.itemCount as int) == 0)
            const Padding(padding: EdgeInsets.all(16), child: Text('No items yet.'))
          else
            Consumer(builder: (context, ref, _) {
              final items = ref.watch(listItemsProvider(list.id as String));
              return AsyncValueView(
                value: items,
                loadingHeight: 80,
                data: (rows) => Column(
                  children: rows
                      .map((item) => ListTile(
                            leading: item.posterPath != null
                                ? ClipRRect(
                                    borderRadius: BorderRadius.circular(6),
                                    child: CachedNetworkImage(
                                      imageUrl: TmdbImages.poster(item.posterPath, size: 'w92')!,
                                      width: 40,
                                      fit: BoxFit.cover,
                                    ),
                                  )
                                : const Icon(Icons.image_not_supported_outlined),
                            title: Text(item.title),
                            onTap: () => context.push(item.isMovie ? '/movie/${item.movieTmdbId}' : '/series/${item.seriesTmdbId}'),
                          ))
                      .toList(),
                ),
              );
            }),
        ],
      ),
    );
  }
}
