class CaseSummary {
  const CaseSummary({
    required this.id,
    required this.title,
    required this.synopsis,
  });

  final String id;
  final String title;
  final String synopsis;

  factory CaseSummary.fromJson(Map<String, dynamic> json) => CaseSummary(
    id: json['id'] as String,
    title: json['title'] as String,
    synopsis: json['synopsis'] as String,
  );
}
