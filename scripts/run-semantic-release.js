const fs = require('fs');
const semanticRelease = require('semantic-release');

(async () => {
    const result = await semanticRelease.default({});
    
    if (!result) {
        fs.writeFileSync('release.env', 'NEW_RELEASE=false\n');
        fs.writeFileSync('release-notes.md', '');
        return;
    }
    
    const { nextRelease } = result;
    fs.writeFileSync('release.env', [
        `VERSION=${nextRelease.version}`,
        'NEW_RELEASE=true',
        `TAG=${nextRelease.gitTag}`,
    ].join('\n') + '\n');
    fs.writeFileSync('release-notes.md', nextRelease.notes || '');
})().catch((err) => { console.error(err); process.exit(1); });