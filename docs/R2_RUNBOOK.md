# Cloudflare R2 operations runbook

Operational procedures for Care Guide private file storage. Commands and values below are placeholders;
never paste production credentials or valid signed URLs into tickets or chat.

See [FILE_STORAGE.md](FILE_STORAGE.md) for architecture and HTTP flow.

## Credential rotation

1. Create a new bucket-scoped read/write R2 token and securely capture its S3 Access Key ID and Secret Access Key.
2. Add the new pair to the production secret manager without deleting the old pair.
3. Deploy/restart all API instances so `Storage__R2__AccessKey` and `Storage__R2__SecretKey` use the new pair.
4. Verify initiation, `PUT`, confirmation (`HEAD`), download (`GET`) and deletion with a disposable test object.
5. Revoke the old token in Cloudflare.
6. Verify error rates and cleanup logs after revocation; record who rotated it and when, never the values.

Pre-signed URLs issued before revocation may remain usable until their short expiration. For a suspected
credential leak, rotate immediately, revoke the exposed token, inspect access/operation metrics and
follow the incident-response process.

## Abandoned uploads

The hosted cleanup service selects only pending, unattached files older than
`FileLifecycle:PendingUploadExpirationMinutes`. It deletes the R2 object first and then the database row.
The routine is idempotent: rerunning it is safe when R2 already has no object.

If abandoned files accumulate:

1. Search application logs for `Abandoned upload cleanup failed and will be retried.`.
2. Check API instance health, R2 credentials, bucket access and database connectivity.
3. Confirm cleanup interval and expiration configuration.
4. Restore the dependency and allow the next scheduled run; do not bulk-delete arbitrary prefixes.
5. Compare pending database records with their opaque storage keys before any manual remediation.

## Failed deletion

`DeletionFailed` means storage deletion failed and the database record was preserved. `DeletionPending`
may also remain if storage deletion succeeded but database deletion failed.

1. Identify the file ID and status from internal diagnostics; do not expose its storage key externally.
2. Confirm there are zero `EventFile` references.
3. Check R2 and database availability and the application error details.
4. Retry the authenticated definitive-delete operation. R2 object deletion is idempotent.
5. Escalate repeated failures; never mark success manually while references or an unknown object state remain.

## Quota and cost monitoring

Monitor in Cloudflare R2 metrics/analytics and billing:

- stored bytes and object count;
- Class A mutations (`PUT`) and Class B reads/metadata operations (`GET`/`HEAD`);
- request/error rates and unexpected traffic growth;
- cleanup and `DeletionFailed` application logs;
- pending files older than their expiration and unreferenced files older than retention.

Cloudflare pricing can change. Consult the [current official pricing page](https://developers.cloudflare.com/r2/pricing/)
rather than copying prices into alerts. R2 currently bills storage and operation classes; storage class
choice may add retrieval/minimum-duration considerations. Set budget alerts from expected object size,
upload frequency, confirmation `HEAD`s and downloads.

## Bucket lifecycle rules

Cloudflare supports bucket lifecycle rules, but they operate independently of Care Guide database
references. Do not configure a broad rule that can delete active attachment objects. Any rule must use a
prefix and retention policy proven compatible with the application lifecycle and approved by privacy,
security and operations owners. See [Cloudflare object lifecycles](https://developers.cloudflare.com/r2/buckets/object-lifecycles/).

## Incident evidence

Record timestamps, environment, affected file IDs/counts, API versions, error categories and remediation.
Do not record patient filenames, object contents, credentials, signed URLs or unnecessary health data.
