## Development

Install [Bun](https://bun.sh/) if you haven't already.

From your terminal:

```sh
bun
```

This installs all the necessary dependencies. Then run:

```sh
bun run dev
```

This starts your app in development mode, rebuilding assets on file changes.

## Deployment

First, build your app for production:

```sh
bun run build
```

Then run the app in production mode:

```sh
bun start
```

Now you'll need to pick a host to deploy it to.

### DIY

If you're familiar with deploying node applications, the built-in Remix app server is production-ready.

Make sure to deploy the output of `remix build`

- `build/`
- `public/build/`
