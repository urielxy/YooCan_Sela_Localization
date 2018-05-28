export default class GoogleMapsComponentForm {
    [key: string]: string;

    constructor(public street_number: string = 'short_name',
        public route: string = 'long_name',
        public locality: string = 'long_name',
        public administrative_area_level_1: string = 'short_name',
        public country: string = 'long_name',
        public postal_code: string = 'short_name') { }
}