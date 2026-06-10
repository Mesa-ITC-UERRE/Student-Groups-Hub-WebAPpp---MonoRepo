import { NavigationClient, type NavigationOptions } from "@azure/msal-browser";
import type { NavigateFunction } from "react-router-dom";

/**
 * Override MSAL's default navigation so it uses React Router instead of
 * window.location — this keeps SPA navigation client-side and prevents
 * full page reloads when MSAL redirects between pages in the app.
 */
export class CustomNavigationClient extends NavigationClient {
  private navigate: NavigateFunction;

  constructor(navigate: NavigateFunction) {
    super();
    this.navigate = navigate;
  }

  async navigateInternal(url: string, options: NavigationOptions) {
    const relativePath = url.replace(window.location.origin, "");
    if (options.noHistory) {
      this.navigate(relativePath, { replace: true });
    } else {
      this.navigate(relativePath);
    }
    return false;
  }
}
