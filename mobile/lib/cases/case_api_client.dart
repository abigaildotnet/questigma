import 'dart:convert';
import 'dart:io';

import 'case_summary.dart';

class CaseApiClient {
  const CaseApiClient(this.baseUrl);

  final Uri baseUrl;

  Future<List<CaseSummary>> listCases() async {
    final client = HttpClient()
      ..connectionTimeout = const Duration(seconds: 10);
    final url = baseUrl.resolve('/api/v1/cases');
    try {
      final request = await client.getUrl(url);
      final response = await request.close();
      if (response.statusCode != HttpStatus.ok) {
        await response.drain<void>();
        throw HttpException(
          'Failed to load cases (${response.statusCode})',
          uri: url,
        );
      }
      final body = await utf8.decoder.bind(response).join();
      final json = jsonDecode(body) as List<dynamic>;
      return json
          .map((item) => CaseSummary.fromJson(item as Map<String, dynamic>))
          .toList();
    } finally {
      client.close();
    }
  }
}
