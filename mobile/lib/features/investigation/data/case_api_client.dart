import 'dart:async';
import 'dart:convert';
import 'dart:io';

import '../domain/case_summary.dart';

class CaseApiClient {
  const CaseApiClient(
    this.baseUrl, {
    this.timeout = const Duration(seconds: 15),
  });

  final Uri baseUrl;
  final Duration timeout;

  Future<List<CaseSummary>> listCases() async {
    final client = HttpClient()
      ..connectionTimeout = const Duration(seconds: 10);
    final url = baseUrl.resolve('/api/v1/cases');
    try {
      return await _readCases(client, url).timeout(timeout);
    } finally {
      client.close(force: true);
    }
  }

  Future<List<CaseSummary>> _readCases(HttpClient client, Uri url) async {
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
  }
}
