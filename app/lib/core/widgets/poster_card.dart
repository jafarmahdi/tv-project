import 'package:flutter/material.dart';

import '../theme/app_theme.dart';

/// A poster tile used in horizontal rails (trending, search results, list items).
class PosterCard extends StatelessWidget {
  final String? posterUrl;
  final String title;
  final String? subtitle;
  final double? rating;
  final VoidCallback? onTap;
  final double width;

  const PosterCard({
    super.key,
    required this.posterUrl,
    required this.title,
    this.subtitle,
    this.rating,
    this.onTap,
    this.width = 140,
  });

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;

    return SizedBox(
      width: width,
      child: InkWell(
        borderRadius: BorderRadius.circular(AppTheme.radiusMd),
        onTap: onTap,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            AspectRatio(
              aspectRatio: 2 / 3,
              child: Container(
                decoration: BoxDecoration(
                  color: scheme.surfaceContainerHigh,
                  borderRadius: BorderRadius.circular(AppTheme.radiusMd),
                ),
                clipBehavior: Clip.antiAlias,
                child: Stack(
                  fit: StackFit.expand,
                  children: [
                    if (posterUrl != null)
                      Image.network(
                        posterUrl!,
                        fit: BoxFit.cover,
                        errorBuilder: (_, _, _) => _placeholder(scheme),
                        loadingBuilder: (context, child, progress) =>
                            progress == null ? child : _placeholder(scheme),
                      )
                    else
                      _placeholder(scheme),
                    if (rating != null && rating! > 0)
                      Positioned(
                        top: 8,
                        right: 8,
                        child: _RatingChip(rating: rating!),
                      ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 8),
            Text(
              title,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(fontWeight: FontWeight.w600),
            ),
            if (subtitle != null)
              Text(
                subtitle!,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(color: scheme.onSurfaceVariant),
              ),
          ],
        ),
      ),
    );
  }

  Widget _placeholder(ColorScheme scheme) => Container(
        color: scheme.surfaceContainerHigh,
        child: Icon(Icons.movie_creation_outlined, color: scheme.onSurfaceVariant, size: 32),
      );
}

class _RatingChip extends StatelessWidget {
  final double rating;
  const _RatingChip({required this.rating});

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 3),
      decoration: BoxDecoration(
        color: Colors.black.withValues(alpha: 0.65),
        borderRadius: BorderRadius.circular(8),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.star_rounded, color: AppTheme.accent, size: 14),
          const SizedBox(width: 2),
          Text(
            rating.toStringAsFixed(1),
            style: const TextStyle(color: Colors.white, fontSize: 12, fontWeight: FontWeight.w600),
          ),
        ],
      ),
    );
  }
}
