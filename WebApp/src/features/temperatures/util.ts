export const DEFAULT_PAGE = 1;
export const DEFAULT_PAGE_LIMIT = 25;
export const MAX_PAGE_LIMIT = 100;

export const DEFAULT_ROWS_PER_PAGE_OPTIONS = [10, 25, 50, 100];

/**
 * Gets the page and limit from a provided URLSearchParams object. If these are not present, or
 * cannot be parsed, the default values (page=1, limit=25) are returned.
 */
export function getPageAndLimitFromUrlSearchParams(urlSearchParams: URLSearchParams) {
    const pageQueryValue = urlSearchParams.get('page');
    const limitQueryValue = urlSearchParams.get('limit');

    const page = pageQueryValue ? parseInt(pageQueryValue) : DEFAULT_PAGE;
    const limit = limitQueryValue ? parseInt(limitQueryValue) : DEFAULT_PAGE_LIMIT;

    return { page, limit };
}
