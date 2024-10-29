docomposer.api = docomposer.api || {};
(function () {
    //import axios from 'axios'
    this.client = null;
    this.setup = async function(timeout = 0) {
        this.client = axios.create({
            baseURL: '/api/',
            timeout: timeout,
            headers: {
                "Authorization": "token " + docomposer.apiToken(),
                "Content-Type": "application/json"
            }
        });
    };
    
}).apply(docomposer.api);